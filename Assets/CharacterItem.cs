using UnityEngine;

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

    // temporary for changing look of characters
    public SpriteRenderer characterSprite;
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
}
