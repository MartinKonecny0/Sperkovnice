using NUnit.Framework;
using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEditor.Search;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public InputSystem_Actions playerInput;
    private InputAction movement;
    public float lastHorizontal = 0f;
    public float horizontal = 0f;

    private InputAction interaction;
    private float lastInteract;
    private float interact;

    public float speed = 1f;
    public GameObject player;
    public PlayerStates playerState = PlayerStates.move;
    public CharacterType currentCharacter;

    public float interactionHeight = 3f;
    private float interactTimer = 0f;
    public float interactCooldown = 0.5f; //time to enter interact state
    private float dropTimer = 0f;
    public float dropCooldown = 1f; //duration for how long player have to hold space to drop item
    public List<GameObject> itemsList;
    private int selectedItemIndex;
    public GameObject itemBar;
    public GameObject[] barIcons = new GameObject[3]; // [0] - previous, [1] - selected, [2] - next

    public SpriteRenderer inventoryIcon;
    public GameObject inventoryItem; //item picked up by player
    public string inventoryItemName;


    public SpriteRenderer characterSprite;

    public enum PlayerStates
    {
        move, //player is fully able to walk
        interact, //player is choosing item to interact with
        stop, //for animations
    }

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
    }

    void Start()
    {
        player = this.gameObject;
    }

    private void OnEnable()
    {
        movement = playerInput.Player.Move;
        movement.Enable();

        interaction = playerInput.Player.Interact;
        interaction.Enable();
    }

    private void Interact()
    {
        interact = interaction.ReadValue<float>();
        //button up
        if (lastInteract > 0 & interact == 0)
        {
            if (playerState == PlayerStates.interact)
            {
                itemsList[selectedItemIndex].GetComponent<InteractableObject>().Interact(gameObject, this);
                //TODO: should stay in interact state
                playerState = PlayerStates.move;
                ClearListOfItems(itemsList);
                CloseItemBar();
            }
        }
        // button hold
        if (interact > 0)
        {
            if (inventoryItem != null)
            {
                dropTimer += Time.deltaTime;
                if (dropTimer >= dropCooldown)
                {
                    DropItem();
                }
            }
        }
        else //interupted hold
        {
            //TODO: could decrease over time
            // dropTimer -= Time.deltaTime
            dropTimer = 0;
        }
        lastInteract = interact;
    }


    /// <summary>
    /// for now simple FSM with two states 
    /// 
    /// MOVE(starting state) <==> INTERACT 
    /// 
    /// </summary>
    private void Update()
    {
        Interact();

        LayerMask layer = LayerMask.GetMask("Interactable");
        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position, transform.up, interactionHeight, layer);

        horizontal = movement.ReadValue<float>();
        // player is in walking state
        if (playerState == PlayerStates.move)
        {
            player.GetComponent<Rigidbody2D>().linearVelocityX = horizontal * speed;
            if (hits.Length > 0) // there is at least one item to interact with
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hits[0].distance, Color.white);
                if (horizontal == 0 && dropTimer == 0)
                {
                    interactTimer += Time.deltaTime;
                    //entering interacting state
                    if (interactTimer >= interactCooldown)
                    {
                        playerState = PlayerStates.interact;
                        CreateListOfItems(hits);
                        ShowItemBar();
                        interactTimer = 0;
                    }
                }
                else //just walking across interactable items
                {
                    interactTimer = 0;
                }
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * interactionHeight, Color.white);
            }
        }
        // player is staying still and choosing item to interact with
        else if (playerState == PlayerStates.interact)
        {
            if (horizontal != 0)
            {
                interactTimer += Time.deltaTime;
                // leaving interacting state
                if (interactTimer >= interactCooldown)
                {
                    ClearListOfItems(itemsList);
                    CloseItemBar();
                    playerState = PlayerStates.move;
                    interactTimer = 0;
                }
            }
            else
            {
                //TODO: player can drop item while in interacting state
                // swapping selected items
                if (lastHorizontal > 0)
                {
                    selectedItemIndex = (selectedItemIndex + 1) % itemsList.Count;
                    UpdateSelected();
                }
                else if (lastHorizontal < 0)
                {
                    selectedItemIndex = (selectedItemIndex - 1) % itemsList.Count;
                    if (selectedItemIndex < 0)
                    {
                        selectedItemIndex = itemsList.Count - 1;
                    }
                    UpdateSelected();
                }

                interactTimer = 0;
            }
            lastHorizontal = horizontal;
        }
    }
    void CreateListOfItems(RaycastHit2D[] hits)
    {
        for (int i = 0; i < hits.Length; i++)
        {
            itemsList.Add(hits[i].collider.gameObject);
        }
    }
    void ClearListOfItems(List<GameObject> list)
    {
        list.Clear();
    }

    void UpdateSelected()
    {
        int itemsCount = itemsList.Count;
        if (itemsCount > 1)
        {
            int nextItemIndex = (selectedItemIndex + 1) % itemsCount;
            barIcons[2].GetComponent<SpriteRenderer>().sprite = itemsList[nextItemIndex].GetComponent<InteractableObject>().icon;
        }
        else
        {
            barIcons[2].GetComponent<SpriteRenderer>().sprite = null;
        }
        if (itemsCount > 2)
        {
            int prevItemIndex = selectedItemIndex - 1;
            if (prevItemIndex < 0)
            {
                prevItemIndex = itemsCount - 1;
            }
            barIcons[0].GetComponent<SpriteRenderer>().sprite = itemsList[prevItemIndex].GetComponent<InteractableObject>().icon;
        }
        else
        {
            barIcons[0].GetComponent<SpriteRenderer>().sprite = null;
        }
        barIcons[1].GetComponent<SpriteRenderer>().sprite = itemsList[selectedItemIndex].GetComponent<InteractableObject>().icon;
    }
    void ShowItemBar()
    {
        selectedItemIndex = 0;
        UpdateSelected();
        itemBar.SetActive(true);
    }
    void CloseItemBar()
    {
        selectedItemIndex = 0;
        itemBar.SetActive(false);
    }

    public void PickUpItem(GameObject item)
    {
        if (inventoryItem == null)
        {
            inventoryItem = item;
            inventoryItemName = item.GetComponent<PickupItem>().itemName;
            inventoryIcon.sprite = item.GetComponent<InteractableObject>().icon;
        }
        else
        {
            DropItem();
            inventoryItem = item;
            inventoryItemName = item.GetComponent<PickupItem>().itemName;
            inventoryIcon.sprite = item.GetComponent<InteractableObject>().icon;
        }
        // item is still present in the scene
        item.SetActive(false);
    }
    /// <summary>
    /// dropping item from inventory and setting their parent room
    /// </summary>
    /// <returns>
    ///     true - sucessfully dropped item
    ///     false - there was no item in inventory
    /// </returns>
    public bool DropItem()
    {
        //TODO: how it should work - where the item should be placed
        // !! wide item can be placed into the wall !!
        if (inventoryItem != null)
        {
            inventoryItem.SetActive(true);
            inventoryItem.transform.position = gameObject.transform.position;
            inventoryItem.transform.parent = transform.parent;
            inventoryItem = null;
            inventoryItemName = "";
            inventoryIcon.sprite = null;
            return true;
        }
        else
        {
            return false;
        }
    }
}
