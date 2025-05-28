using System;
using NUnit.Framework.Constraints;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using static Player;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Cache = Unity.VisualScripting.Cache;

public enum CharacterType
{
    Hedwig, // Hedvika
    Engineer, // Topiè
    Shaman, // Šaman
}
public class CharacterItem : InteractableObject
{
    public RoomManager roomManager;
    public CharacterType characterType;

    public bool isWalkingToDefault = false;
    public Transform defaultPosition;
    public float walkSpeed = 2;
    // temporary for changing look of characters
    public SpriteRenderer characterSprite;

    void Start()
    {
        type = InteractableType.character;
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        CharacterType previousCharacterType = playerScript.currentCharacter;
        // temporary for changing look of characters

        //Player newPlayer = CopyComponent(playerScript, this.gameObject);
        playerScript.player = this.gameObject;
        playerScript.gameObject.transform.parent = this.gameObject.transform;
        playerScript.gameObject.transform.position = this.gameObject.transform.position;
        this.gameObject.GetComponent<Collider2D>().isTrigger = false;
        playerScript.currentCharacter = characterType;
        this.gameObject.layer = 0; // default layer


        character.GetComponent<Collider2D>().isTrigger = true;
        character.layer = 6; // interactable layer
        roomManager.ChangeWorld(characterType);
    }

    void Update()
    {
        if (isWalkingToDefault)
        {
            float deltaDefault = 0.1f;
            float horizontal;

            float distance = defaultPosition.position.x - transform.position.x;

            if (Mathf.Abs(distance) >= deltaDefault)
            {
                if (defaultPosition.position.x - transform.position.x < 0)
                {

                    horizontal = -1;
                }
                else
                {
                    horizontal = 1;
                }
                GetComponent<Rigidbody2D>().linearVelocityX = horizontal * walkSpeed;
            }
            else
            {
                GetComponent<Rigidbody2D>().linearVelocityX = 0;
                isWalkingToDefault = false;
            }
        }
    }
}
