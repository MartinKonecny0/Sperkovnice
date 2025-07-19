using System;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static PickupItem;
using static UnityEngine.UI.Image;

public class TaskObject : InteractableObject
{
    public enum TaskObjectType
    {
        Pulley_Rope,
        Pulley_Coal,
        Ignite,
        Brew,
        Weapon
    }

    private RoomManager roomManager;
    public TaskObjectType itemType;
    public bool isCompleted = false;
    public ForcedAction taskCompletedActions;
    public List<PickupItem.PickupItemType> requiredItems;
    public List<PickupItem.PickupItemType> itemsLeft;
    private PickupItem.PickupItemType playerItem;
    public bool isRevertable;
    public bool needItemForRevert;
    public PickupItemType revertItem;
    void Start()
    {
        roomManager = GameObject.Find("GameManager").GetComponent<RoomManager>();
        taskCompletedActions = GetComponent<ForcedAction>();
        type = InteractableType.task;
        itemsLeft = new List<PickupItemType>(requiredItems);
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        if (playerScript.inventoryItem != null)
        {
            playerItem = playerScript.inventoryItem.GetComponent<PickupItem>().itemType;
        }
        else
        {
            //playerItem = null;
        }
        // player can reuse items in some tasks (for example rope in pulley task)
        if (isRevertable && isCompleted)
        {
            if (!needItemForRevert || playerItem == revertItem)
            {
                RevertTask(playerScript);
            }
            else
            {
                Debug.Log("Holding wrong item for revert");
            }
        }
        CheckPlayerItem(playerScript);
    }
    private void CheckPlayerItem(Player playerScript)
    {
        if (playerScript.inventoryItem == null)
        {
            Debug.Log("Player is holding nothing");
            return;
        }
        // item is required
        if (itemsLeft.Contains(playerItem))
        {
            Debug.Log("Player is holding object for this task");
            DestroyPlayerItem(playerScript);
            if (itemsLeft.Count == 0)
            {
                TaskCompleted();
            }
        }
        else
        {
            Debug.Log("Player is NOT holding object for this task");
        }
    }

    private void TaskCompleted()
    {
        isCompleted = true;
        if (!isRevertable)
        {
            isInteractable = false;
        }
        if (taskCompletedActions != null)
        {
            taskCompletedActions.ExecuteAction();
        }
        
    }

    private void RevertTask(Player playerScript)
    {
        DestroyPlayerItem(playerScript);

        isCompleted = false;
        itemsLeft = new List<PickupItemType>(requiredItems);
        taskCompletedActions.RevertAction();

        foreach (PickupItemType itemToRevert in requiredItems)
        {
            GameObject newItem = roomManager.GetItemPrefabByType(itemToRevert);
            GameObject spawnedItem = Instantiate(newItem, transform.position, transform.rotation, transform.parent);
            roomManager.allItems.Add(spawnedItem.GetComponent<PickupItem>());
        }
    }

    private void DestroyPlayerItem(Player playerScript)
    {
        if (playerScript.inventoryItem != null)
        {
            itemsLeft.Remove(playerItem);
            playerScript.inventoryIcon.sprite = null;
            roomManager.allItems.Remove(playerScript.inventoryItem.GetComponent<PickupItem>());
            Destroy(playerScript.inventoryItem);
        }
    }
}
