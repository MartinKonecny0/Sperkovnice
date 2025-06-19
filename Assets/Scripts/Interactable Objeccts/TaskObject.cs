using System.Collections.Generic;
using UnityEngine;
using static PickupItem;

public class TaskObject : InteractableObject
{
    public int id;
    public bool isCompleted = false;
    public ForcedAction taskCompletedActions;
    public List<PickupItem.PickupItemType> requiredItems;
    private PickupItem.PickupItemType playerItem;

    void Start()
    {
        taskCompletedActions = GetComponent<ForcedAction>();
        type = InteractableType.task;
    }

    public override void Interact(GameObject character, Player playerScript)
    {
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
        if (requiredItems.Contains(playerItem))
        {
            Debug.Log("Player is holding object for this task");
            requiredItems.Remove(playerItem);
            playerScript.inventoryIcon.sprite = null;
            Destroy(playerScript.inventoryItem);
            
            if (requiredItems.Count == 0)
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
        isInteractable = false;
        if (taskCompletedActions != null)
        {
            taskCompletedActions.ExecuteAction();
        }
        
    }
}
