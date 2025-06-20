using UnityEngine;

public class ForcedAction : MonoBehaviour
{
    private RoomManager roomManager;
    public int[] itemIDsToActivate;
    public GameObject[] objectsToActivate;
    public Dialog[] dialogsToEnable;
    public Dialog[] dialogsToDisable;


    void Start()
    {
        roomManager = GameObject.Find("GameManager").GetComponent<RoomManager>();
    }
    public void ExecuteAction()
    {
        // pickup items 
        foreach (int itemId in itemIDsToActivate)
        {
            PickupItem pickupItem = roomManager.GetItemInstanceById(itemId);
            pickupItem.gameObject.SetActive(true);
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
        // pickup items 
        foreach (int itemId in itemIDsToActivate)
        {
            PickupItem pickupItem = roomManager.GetItemInstanceById(itemId);
            pickupItem.gameObject.SetActive(false);
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
