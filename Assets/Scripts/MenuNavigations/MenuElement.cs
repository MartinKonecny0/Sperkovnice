using UnityEngine;

public abstract class MenuElement : MonoBehaviour
{
    public abstract void Interact();
    public abstract void Select();
    public abstract void Deselect();
}
