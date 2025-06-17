using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public enum InteractableType {
        character,
        movement,
        pickup,
        task,
        puzzle,
    }
    public Sprite icon;
    public InteractableType type;
    public bool isInteractable;

    public abstract void Interact(GameObject character, Player playerScript);
     // idea public virtual bool IsInteractable()
     // {
     // return isInteractable; // override in movementItem and pickupItem
     // }
     //
     //
}
