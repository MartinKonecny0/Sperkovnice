using System;
using System.Collections.Generic;
using UnityEngine;
using static PickupItem;
using static UnityEngine.UI.Image;

public class TaskObject : InteractableObject
{
    public int id;
    public bool isCompleted = false;
    public ForcedAction taskCompletedActions;
    public List<PickupItem.PickupItemType> requiredItems;
    private List<PickupItem.PickupItemType> itemsLeft;
    private PickupItem.PickupItemType playerItem;
    public bool isRevertable;
    void Start()
    {
        taskCompletedActions = GetComponent<ForcedAction>();
        type = InteractableType.task;
        itemsLeft = new List<PickupItemType>(requiredItems);
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        // player can reuse items in some tasks (for example rope in pulley task)
        if (isRevertable && isCompleted)
        {
            RevertTask();
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

        playerItem = playerScript.inventoryItem.GetComponent<PickupItem>().itemType;
        
        // item is required
        if (itemsLeft.Contains(playerItem))
        {
            Debug.Log("Player is holding object for this task");
            itemsLeft.Remove(playerItem);
            playerScript.inventoryIcon.sprite = null;
            
            Destroy(playerScript.inventoryItem);
           
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

    private void RevertTask()
    {
        isCompleted = false;
        itemsLeft = new List<PickupItemType>(requiredItems);
        taskCompletedActions.RevertAction();

        RoomManager roomManager = GameObject.Find("GameManager").GetComponent<RoomManager>();
        foreach (PickupItemType itemToRevert in requiredItems)
        {
            GameObject newItem = roomManager.GetItemPrefabByType(itemToRevert);
            Instantiate(newItem, transform.position, transform.rotation, transform.parent);
        }
    }
}
