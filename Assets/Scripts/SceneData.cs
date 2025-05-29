using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneData
{
    [System.Serializable]
    public struct playerSave
    {
        public float[] position;
        public CharacterType character;
        public string roomName;
        public string playerCurrentItem;
    }
    [System.Serializable]
    public struct pickupItemSave
    {
        public float[] position;
        public string name;
        public string roomName;
    }

    [System.Serializable]
    public struct characterSave
    {
        public float[] position;
        public CharacterType character;
        public string roomName;

        // current dialog/task state
    }

    public playerSave playerData;
    public pickupItemSave[] allPickupItems;
    public characterSave[] allCharacters;

    public SceneData(Player player, PickupItem[] items, CharacterItem[] characters)
    {
        playerData = new playerSave();
        playerData.position = new float[2];
        playerData.position[0] = player.transform.position.x;
        playerData.position[1] = player.transform.position.y;
        playerData.character = player.currentCharacter;
        playerData.roomName = player.transform.parent.name;
        playerData.playerCurrentItem = player.inventoryItemName;



        //set all pickup items in scene
        int i = 0;
        allPickupItems = new pickupItemSave[items.Length];
        foreach (var item in items)
        {
            pickupItemSave newItem = new pickupItemSave();
            newItem.position = new float[2];
            newItem.position[0] = item.transform.position.x;
            newItem.position[1] = item.transform.position.y;
            newItem.name = item.itemName;
            newItem.roomName = item.transform.parent.name;
            allPickupItems[i] = newItem;
            i++;
        }

        i = 0;
        allCharacters = new characterSave[characters.Length];
        foreach (var character in characters)
        {
            characterSave newCharacter = new characterSave();
            newCharacter.position = new float[2];
            newCharacter.position[0] = character.transform.position.x;
            newCharacter.position[1] = character.transform.position.y;
            newCharacter.character = character.characterType;
            newCharacter.roomName = character.transform.parent.name;
            allCharacters[i] = newCharacter;
            i++;
        }
    }
}
