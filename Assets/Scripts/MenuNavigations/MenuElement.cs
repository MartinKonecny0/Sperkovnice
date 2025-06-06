using UnityEngine;

public abstract class MenuElement : MonoBehaviour
{
    public abstract void Interact();
    public abstract void Select();
    public abstract void Deselect();
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        MenuManager manager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        MenuElement selectedElement = manager.allSelectableButtons[manager.selectedButtonIndex];
        if (this == selectedElement)
        {
            manager.SelectPreviousButton();
        }
    }
}
