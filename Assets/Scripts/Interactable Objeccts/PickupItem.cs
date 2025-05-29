using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : InteractableObject
{
    // dangerous duplicate items while saving and loading
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
