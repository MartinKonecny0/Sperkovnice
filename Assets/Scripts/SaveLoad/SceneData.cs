using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class SceneData
{
    [System.Serializable]
    public struct metaSave
    {
        public string saveDate;
    }
    [System.Serializable]

    public struct playerSave
    {
        public CharacterType character;
        public int playerCurrentItemId;
    }
    [System.Serializable]
    public struct pickupItemSave
    {
        public float[] position;
        public int id;
        public string name;
        public string roomName;
    }

    [System.Serializable]
    public struct characterSave
    {
        public float[] position;
        public CharacterType character;
        public bool isWalkingToDefault;
        public string roomName;

        // current dialog/task state
    }

    public metaSave metaData; 
    public playerSave playerData;
    public List<pickupItemSave> allPickupItems = new List<pickupItemSave>();
    public List<characterSave> allCharacters = new List<characterSave>();

    public SceneData(Player player, List<PickupItem> items, List<CharacterItem> characters)
    {
        metaData = new metaSave();
        metaData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        playerData = new playerSave();
        playerData.character = player.currentCharacter;
        if (player.inventoryItem != null)
        {
            playerData.playerCurrentItemId = player.inventoryItem.GetComponent<PickupItem>().id;
        }
        else
        {
            playerData.playerCurrentItemId = 0;
        }


           //set all pickup items in scene
        int i = 0;
        foreach (var item in items)
        {
            if (item != null)
            {
                pickupItemSave newItem = new pickupItemSave();
                newItem.position = new float[2];
                newItem.position[0] = item.transform.position.x;
                newItem.position[1] = item.transform.position.y;
                newItem.id = item.id;
                newItem.name = item.itemName;
                newItem.roomName = item.transform.parent.name;
                allPickupItems.Add(newItem);
                i++;
            }
        }

        i = 0;
        foreach (var character in characters)
        {
            if (character != null)
            {
                characterSave newCharacter = new characterSave();
                newCharacter.position = new float[2];
                newCharacter.position[0] = character.transform.position.x;
                newCharacter.position[1] = character.transform.position.y;
                newCharacter.character = character.characterType;
                newCharacter.isWalkingToDefault = character.isWalkingToDefault;
                newCharacter.roomName = character.transform.parent.name;
                allCharacters.Add(newCharacter);
                i++;
            }
        }
    }
}
