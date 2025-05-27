using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : InteractableObject
{
    public string itemName;
    void Start()
    {
        type = InteractableType.pickup;
    }
    public override void Interact(GameObject character, Player playerScript)
    {
        playerScript.PickUpItem(gameObject);
    }
}
