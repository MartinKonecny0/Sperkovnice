using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TaskObject : InteractableObject
{
    public List<string> requiredItemsNames;
    private string playerItemName;

    void Start()
    {
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
        
        playerItemName = playerScript.GetComponent<PickupItem>().itemName;
        
        // item is required
        if (requiredItemsNames.Contains(playerItemName))
        {
            Debug.Log("Player is holding object for this task");
            requiredItemsNames.Remove(playerScript.GetComponent<PickupItem>().itemName);
            playerScript.inventoryIcon.sprite = null;
            Destroy(playerScript.inventoryItem);
        }
        else
        {
            Debug.Log("Player is NOT holding object for this task");
        }
    }
}
