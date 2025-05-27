using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Playables;
using static Player;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

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
        Color previousColor = playerScript.characterSprite.color;
        playerScript.characterSprite.color = characterSprite.color;
        characterSprite.color = previousColor;


        playerScript.currentCharacter = characterType;
        roomManager.ChangeWorld(characterType);
        characterType = previousCharacterType;
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
