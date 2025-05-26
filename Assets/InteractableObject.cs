using UnityEngine;

public abstract class InteractableObject : MonoBehaviour
{
    public enum InteractableType {
        character,
        movement,
        item,
        enviroment,
    }
    public Sprite icon;
    public InteractableType type;


    public abstract void Interact(GameObject character, Player playerScript);
}
