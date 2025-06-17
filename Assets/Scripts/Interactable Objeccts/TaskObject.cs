using System.Collections.Generic;
using UnityEngine;

public class TaskObject : InteractableObject
{
    public int id;
    public bool isCompleted = false;
    public ForcedAction taskCompletedActions;
    public List<string> requiredItemsNames;
    private string playerItemName;

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

        playerItemName = playerScript.inventoryItem.GetComponent<PickupItem>().itemName;
        
        // item is required
        if (requiredItemsNames.Contains(playerItemName))
        {
            Debug.Log("Player is holding object for this task");
            requiredItemsNames.Remove(playerItemName);
            playerScript.inventoryIcon.sprite = null;
            Destroy(playerScript.inventoryItem);
            if (requiredItemsNames.Count == 0)
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
