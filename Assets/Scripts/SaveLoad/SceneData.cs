using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
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
        public PickupItem.PickupItemType playerCurrentItemType;
        public int playerCurrentItemId;
    }
    [System.Serializable]
    public struct characterSave
    {
        public float[] position;
        public CharacterType character;
        public bool isWalkingToDefault;
        public string roomName;

        public int[] dialogsToSay;
        // current dialog/task state
    }
    [System.Serializable]
    public struct pickupItemSave
    {
        public float[] position;
        public bool isActive;
        public PickupItem.PickupItemType itemType;
        public int id; // different for each instance
        public bool isInteractable;
        public string roomName;
    }
    [System.Serializable]
    public struct taskItemSave
    {
        public TaskObject.TaskObjectType itemType;
        public bool isActive;
        public bool isInteractable;
        public bool isCompleted;
        public PickupItem.PickupItemType[] remainingRequiredItems;
    }
    [System.Serializable]
    public struct itemWithRequirement
    {
        public ItemWithRequirement.ItemWithRequirementType itemType;
        public bool isActive;
        public bool isInteractable;
        public bool isCompleted;
        public bool isNecessaryDiscovered;
    }

    public metaSave metaData; 
    public playerSave playerData;
    public List<pickupItemSave> allPickupItems = new List<pickupItemSave>();
    public List<characterSave> allCharacters = new List<characterSave>();
    public List<taskItemSave> allTaskItems = new List<taskItemSave>();
    public List<itemWithRequirement> allItemsWithRequirement = new List<itemWithRequirement>();

    public SceneData(Player player, List<PickupItem> items, List<CharacterItem> characters, List<TaskObject> tasks, List<ItemWithRequirement> itemsWithReq)
    {
        metaData = new metaSave();
        metaData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        playerData = new playerSave();
        playerData.character = player.currentCharacter;
        if (player.inventoryItem != null)
        {
            playerData.playerCurrentItemId = player.inventoryItem.GetComponent<PickupItem>().id;
            playerData.playerCurrentItemType = player.inventoryItem.GetComponent<PickupItem>().itemType;
        }
        else
        {
            playerData.playerCurrentItemId = -1;
        }

        //set all pickup items in scene
        foreach (var item in items)
        {
            if (item != null)
            {
                pickupItemSave newItem = new pickupItemSave();
                newItem.isActive = item.gameObject.activeSelf;
                newItem.position = new float[2];
                newItem.position[0] = item.transform.position.x;
                newItem.position[1] = item.transform.position.y;
                newItem.id = item.id;
                newItem.isInteractable = item.isInteractable;
                newItem.itemType = item.itemType;
                newItem.roomName = item.transform.parent.name;
                allPickupItems.Add(newItem);
            }
        }

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
            }
        }
        foreach (var task in tasks)
        {
            if (task != null)
            {
                taskItemSave newTask = new taskItemSave();
                newTask.isActive = task.gameObject.activeSelf;
                newTask.itemType = task.itemType;
                newTask.isInteractable = task.isInteractable;
                newTask.isCompleted = task.isCompleted;
                newTask.remainingRequiredItems = new PickupItem.PickupItemType[task.itemsLeft.Count];
                for (int i = 0; i < newTask.remainingRequiredItems.Length; i++)
                {
                    newTask.remainingRequiredItems[i] = task.itemsLeft[i];
                }
                allTaskItems.Add(newTask);
            }
        }

        foreach (var item in itemsWithReq)
        {
            if (item != null)
            {
                itemWithRequirement newItem = new itemWithRequirement();
                newItem.isActive = item.gameObject.activeSelf;
                newItem.itemType = item.itemType;
                newItem.isInteractable = item.isInteractable;
                newItem.isCompleted = item.isCompleted;
                newItem.isNecessaryDiscovered = item.isNecessaryDiscovered;
                allItemsWithRequirement.Add(newItem);
            }
        }
    }
}
