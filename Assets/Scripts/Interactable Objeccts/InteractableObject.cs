using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public enum InteractableType {
        character,
        movement,
        pickup,
        task,
        puzzle,
        itemWithRequirement
    }
    public Sprite icon;
    public InteractableType type;
    public bool isInteractable;

    public abstract void Interact(GameObject character, Player playerScript);
    public virtual bool IsInteractable()
    {
        return isInteractable; // override in pickupItem
    }


}
