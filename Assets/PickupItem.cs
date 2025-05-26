using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : InteractableObject
{
    public override void Interact(GameObject character, Player playerScript)
    {
        playerScript.PickUpItem(gameObject);
    }
}
