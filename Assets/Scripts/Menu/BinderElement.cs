using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public class BinderElement : MenuElement
{
    public enum binderType
    {
        left,
        right,
        interact,
        escape
    }

    public binderType currentBinderType;
    public MenuManager menuManager;

    // when panel with this is enabled creates waiter for callback and changes the player Input
    void OnEnable()
    {
        // creates waiting callback 
        InputSystem.onAnyButtonPress.CallOnce(currentAction =>
        {
            if (currentAction is ButtonControl button)
            {
                string bindingString = $"<{currentAction.device.name}>/" + currentAction.name;
                menuManager.ChangeBinding(currentBinderType, bindingString);

                menuManager.SaveSettings();

                menuManager.skipNextButtonUp = true;
                menuManager.RemovePanel();
            }
        });
    }

    public override void Interact()
    {
    }

    public override void Select()
    {
    }

    public override void Deselect()
    {
    }
}
