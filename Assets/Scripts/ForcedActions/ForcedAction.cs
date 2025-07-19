using UnityEngine;

public class ForcedAction : MonoBehaviour
{
    private RoomManager roomManager;
    public PickupItem.PickupItemType[] pickupInstancesToSpawn;
    public GameObject[] objectsToActivate;
    public Dialog[] dialogsToEnable;
    public Dialog[] dialogsToDisable;
    public Transform spawningPosition;
    void Start()
    {
        roomManager = GameObject.Find("GameManager").GetComponent<RoomManager>();
    }
    public void ExecuteAction()
    {
        // spawn pickup items - always spawns with id 0
        // not suitable for spawning multiple items with same type (for example 2 and more tomatoes)
        foreach (PickupItem.PickupItemType itemId in pickupInstancesToSpawn)
        {
            GameObject spawnedItem = roomManager.GetItemPrefabByType(itemId);
            Instantiate(spawnedItem, spawningPosition.position, spawningPosition.rotation, spawningPosition.parent);
            roomManager.allItems.Add(spawnedItem.GetComponent<PickupItem>());
        }

        // task objects etc.  (objects that are not saved and spawned)
        foreach (GameObject objectToEnable in objectsToActivate)
        {
            objectToEnable.SetActive(true);
        }

        // dialogs
        foreach (Dialog dialog in dialogsToDisable)
        {
            Destroy(dialog);
        }

        foreach (Dialog dialog in dialogsToEnable)
        {   // TODO: only if dialog can be activated (was not destroyed)
            CharacterItem speakerItem = roomManager.GetCharacterInstanceByType(dialog.speaker);
            speakerItem.AddDialogToSay(dialog);
        }
    }
    public void RevertAction()
    {
        // destroying pickup items 
        // not suitable for destroying items that exists in multiple instances (for example 2 and more tomatoes)
        foreach (PickupItem.PickupItemType itemId in pickupInstancesToSpawn)
        {
            PickupItem pickupItem = roomManager.GetItemInstanceById(itemId, 0);
            Destroy(pickupItem);
        }

        // task objects etc.  (objects that are not saved and spawned)
        foreach (GameObject objectToEnable in objectsToActivate)
        {
            objectToEnable.SetActive(false);
        }

        // dialogs
        //foreach (Dialog dialog in dialogsToDisable)
        //{
        //    Destroy(dialog);
        //}

        //foreach (Dialog dialog in dialogsToEnable)
        //{   // TODO: only if dialog can be activated (was not destroyed)
        //    CharacterItem speakerItem = roomManager.GetCharacterInstanceByType(dialog.speaker);
        //    speakerItem.AddDialogToSay(dialog);
        //}
    }
}
