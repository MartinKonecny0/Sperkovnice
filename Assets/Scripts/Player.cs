using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using NUnit.Framework;

public class Player : MonoBehaviour
{
    public RoomManager roomManager;

    public InputSystem_Actions playerInput;
    private InputAction movement;
    public float lastHorizontal = 0f;
    public float horizontal = 0f;
    private int lastNumOfItems;
    private InputAction interaction;
    private float lastInteract;
    private float interact;
    private InputAction escape;
    private float lastBack;
    private float back;

    public float speed = 1f;
    public GameObject player;
    public PlayerStates playerState = PlayerStates.move;
    public CharacterType currentCharacter;

    public float interactionHeight = 3f;
    public float interactionWidth = 10f; // used for detecting walls while dropping items

    private float interactTimer = 0f;
    public float interactCooldown = 0.5f; //time to enter interact state
    private float dropTimer = 0f;
    public float dropCooldown = 1f; //duration for how long player have to hold space to drop item
    public List<GameObject> itemsList;
    private int selectedItemIndex;
    public ItemBar itemBar;

    public SpriteRenderer inventoryIcon;
    public float leftWallDistance;
    public float rightWallDistance;
    public GameObject inventoryItem; //item picked up by player

    public SpriteRenderer characterSprite;

    public enum PlayerStates
    {
        move, //player is fully able to walk
        interact, //player is choosing item to interact with
        stop, //for animations
    }

    private void Awake()
    {
        playerInput = BindingManager.playerInput;
    }

    void Start()
    {
        SaveManager.player = this;
        roomManager = GameObject.Find("GameManager").GetComponent<RoomManager>();
        inventoryIcon = roomManager.inventoryIcon;
    }

    private void OnEnable()
    {
        movement = playerInput.Player.Move;
        movement.Enable();

        interaction = playerInput.Player.Interact;
        interaction.Enable();

        escape = playerInput.Player.Escape;
        escape.Enable();
    }

    private void Interact()
    {
        interact = interaction.ReadValue<float>();
        back = escape.ReadValue<float>();
        //button up
        if (lastInteract > 0 & interact == 0)
        {
            if (playerState == PlayerStates.interact)
            {
                itemsList[selectedItemIndex].GetComponent<InteractableObject>().Interact(player, this);
                //TODO: should stay in interact state
                playerState = PlayerStates.move;
                itemBar.HideBar();
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
        hits = Physics2D.RaycastAll(player.transform.position, transform.up, interactionHeight, layer);

        CreateListOfItems(hits);

        horizontal = movement.ReadValue<float>();
        // player is in walking state
        if (playerState == PlayerStates.move)
        {
            player.GetComponent<Rigidbody2D>().linearVelocityX = horizontal * speed;
            if (itemsList.Count > 0) // there is at least one item to interact with
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hits[0].distance, Color.white);
                if (horizontal == 0 && dropTimer == 0)
                {
                    interactTimer += Time.deltaTime;
                    //entering interacting state
                    if (interactTimer >= interactCooldown)
                    {
                        playerState = PlayerStates.interact;
                        selectedItemIndex = 0;
                        itemBar.CreateBar(itemsList);
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
                Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.up) * interactionHeight, Color.white);
            }
        }
        // player is staying still and choosing item to interact with
        else if (playerState == PlayerStates.interact)
        {
            // special update of item bar when number of items player can interact with changes
            if (lastNumOfItems != itemsList.Count)
            {
                itemBar.UpdateBar(itemsList);
            }
            if (horizontal != 0)
            {
                interactTimer += Time.deltaTime;
                // leaving interacting state
                if (interactTimer >= interactCooldown)
                {
                    itemBar.HideBar();
                    playerState = PlayerStates.move;
                    interactTimer = 0;
                }
            }
            else
            {
                // swapping selected items
                if (lastHorizontal > 0)
                {
                    selectedItemIndex = (selectedItemIndex + 1) % itemsList.Count;
                    itemBar.RotateBarRight(selectedItemIndex);
                }
                else if (lastHorizontal < 0)
                {
                    selectedItemIndex = (selectedItemIndex - 1) % itemsList.Count;
                    if (selectedItemIndex < 0)
                    {
                        selectedItemIndex = itemsList.Count - 1;
                    }
                    itemBar.RotateBarLeft(selectedItemIndex);

                }

                interactTimer = 0;
            }
        }

        if (back > 0)
        {
            roomManager.Save();
            roomManager.ExitToMenu();
        }

        lastNumOfItems = itemsList.Count;
        lastHorizontal = horizontal;
    }
    void CreateListOfItems(RaycastHit2D[] hits)
    {
        itemsList.Clear();

        for (int i = 0; i < hits.Length; i++)
        {
            GameObject hitObject = hits[i].collider.gameObject;
            if (hitObject.GetComponent<InteractableObject>().IsInteractable())
            {
                itemsList.Add(hits[i].collider.gameObject);
            }
        }
    }

    public void PickUpItem(GameObject item)
    {
        if (inventoryItem == null)
        {
            inventoryItem = item;
            inventoryIcon.sprite = item.GetComponent<InteractableObject>().icon;
        }
        else
        {
            DropItem();
            inventoryItem = item;
            inventoryIcon.sprite = item.GetComponent<InteractableObject>().icon;
        }
        roomManager.UpdateNecessaryItems(item.GetComponent<PickupItem>().itemType);
        // item is still present in the scene
        item.SetActive(false);
    }
    /// <summary>
    /// dropping item from inventory and setting their parent room
    /// </summary>
    /// <returns>
    ///     true - successfully dropped item
    ///     false - there was no item in inventory
    /// </returns>
    public bool DropItem()
    {
        if (inventoryItem != null)
        {
            GetWallsDistances();
            float itemWidth = inventoryItem.GetComponent<PickupItem>().itemWidth;
            float offsetFromWall = 0;

            // if items would be placed into the wall calculate offset so they will be placed next to the wall
            if (itemWidth / 2 > rightWallDistance)
            {
                offsetFromWall = -Mathf.Abs(itemWidth / 2 - rightWallDistance);
            }

            if (itemWidth / 2 > leftWallDistance)
            {
                offsetFromWall = Mathf.Abs(itemWidth / 2 - leftWallDistance);
            }

            inventoryItem.SetActive(true);
            inventoryItem.transform.position = player.transform.position + new Vector3(offsetFromWall, 0, 0);
            inventoryItem.transform.parent = player.transform.parent;
            inventoryItem = null;
            inventoryIcon.sprite = null;
            playerState = PlayerStates.move;
            itemBar.HideBar();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void GetWallsDistances()
    {
        LayerMask layer = LayerMask.GetMask("Walls");
        var rightHit = Physics2D.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), transform.right, interactionWidth, layer);
        rightWallDistance = rightHit.distance;
        var leftHit = Physics2D.Raycast(player.transform.position + new Vector3(0, 0.5f, 0), -transform.right, interactionWidth, layer);
        leftWallDistance = leftHit.distance;
    }

    public void ChangePlayerToCharacter(GameObject oldCharacter, GameObject character)
    {
        character.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1; // visual layer
        character.layer = 0; // default layer - interaction layer
        character.GetComponent<Collider2D>().isTrigger = false;

        gameObject.transform.parent = character.transform;
        player = character.gameObject;
        gameObject.transform.position = character.transform.position;
        currentCharacter = character.GetComponent<CharacterItem>().characterType;

        if (oldCharacter != null)
        {
            oldCharacter.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
            oldCharacter.GetComponent<Collider2D>().isTrigger = true;
            oldCharacter.layer = 6; // interactable layer
        }
        roomManager.ChangeWorld(currentCharacter);
    }
}
