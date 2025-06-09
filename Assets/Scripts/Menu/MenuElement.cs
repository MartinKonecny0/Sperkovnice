using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public abstract class MenuElement : MonoBehaviour
{
    public float interactHoldCooldown;
    public abstract void Interact();

    public virtual void Right()
    {
        MenuManager manager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        manager.SelectNextButton();
    }
    public virtual void Left()
    {
        MenuManager manager = GameObject.Find("Canvas").GetComponent<MenuManager>();
        manager.SelectPreviousButton();
    }
    public virtual void RightHold()
    {
        // default does nothing - differs in each child element
    }
    public virtual void LeftHold()
    {
        // default does nothing - differs in each child element
    }

    public virtual void HoldInteract()
    {
        // default does nothing - differs in each child element
    }

    public abstract void Select();
    public abstract void Deselect();
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
        //MenuElement selectedElement = manager.currSelectElements[manager.currSelected];
        //if (this == selectedElement)
        //{
        //    manager.SelectPreviousButton();
        //}
    }
}