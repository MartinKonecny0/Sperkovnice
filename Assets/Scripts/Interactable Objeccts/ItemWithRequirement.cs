using UnityEngine;
using static PickupItem;

public class ItemWithRequirement : InteractableObject
{
    public int id;
    public bool isCompleted = false;
    public ForcedAction taskCompletedActions;
    public PickupItemType necessaryItem;
    public bool isNecessaryDiscovered;
    void Start()
    {
        taskCompletedActions = GetComponent<ForcedAction>();
        type = InteractableType.itemWithRequirement;
    }
    public override bool IsInteractable()
    {
        // if necessary item was already touched -> interaction with item is possible
        return isNecessaryDiscovered & isInteractable;
    }
    public void NecessaryItemDiscovered()
    {
        isNecessaryDiscovered = true;
    }
    public override void Interact(GameObject character, Player playerScript)
    {

        if (playerScript.inventoryItem == null)
        {
            Debug.Log("Player is trying to get item that needs another item to be picked up. Required item: " + necessaryItem);
            return;
        }
        else if (playerScript.inventoryItem.GetComponent<PickupItem>().itemType != necessaryItem)
        {
            Debug.Log("Player is trying to get item that needs another item to be picked up. Required item: " + necessaryItem);
            return;
        }
        // correct item in player`s hand
        UnlockItem();
    }
    private void UnlockItem()
    {
        isCompleted = true;
        isInteractable = false;
        if (taskCompletedActions != null)
        {
            taskCompletedActions.ExecuteAction();
        }
    }
}
