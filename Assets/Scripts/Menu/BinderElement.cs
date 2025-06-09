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

    public List<string> allBindingStrings;
    public binderType currentBinderType;
    public MenuManager menuManager;

    // when panel with this is enabled creates waiter for callback and changes the player Input
    void OnEnable()
    {
        GetAllBindingString();
        // creates waiting callback 
        InputSystem.onAnyButtonPress.CallOnce(currentAction =>
        {
            if (currentAction is ButtonControl button)
            {
                ChangeBinding(currentAction);
                menuManager.skipNextButtonUp = true;
            }
            menuManager.RemovePanel();
        });
    }

    public void ChangeBinding(InputControl currentAction)
    {
        menuManager.playerInput.Disable();

        string bindingString = $"<{currentAction.device.name}>/" + currentAction.name;
        if (allBindingStrings.Contains(bindingString))
        {
            Debug.LogError("Cant bind one key to two different actions");
            menuManager.playerInput.Enable();

            return;
        }


        if (currentBinderType == binderType.left)
        {
            string rightBindString = menuManager.playerInput.Player.Move.bindings[2].path;
            for (int i = 0; i < menuManager.playerInput.Player.Move.bindings.Count; i++)
            {
                menuManager.playerInput.Player.Move.ChangeBinding(i).Erase();
            }
            Debug.Log(rightBindString + " and " + bindingString);

            //menuManager.playerInput.Player.Move.ChangeBinding()
            menuManager.playerInput.Player.Move.AddCompositeBinding("Axis")
                .With("Negative", bindingString)
                .With("Positive", rightBindString);
        }
        else if (currentBinderType == binderType.right)
        {
            string leftBindString = menuManager.playerInput.Player.Move.bindings[1].path;
            Debug.Log(leftBindString + " and " + bindingString);

            for (int i = 0; i < menuManager.playerInput.Player.Move.bindings.Count; i++)
            {
                menuManager.playerInput.Player.Move.ChangeBinding(i).Erase();
            }

            menuManager.playerInput.Player.Move.AddCompositeBinding("Axis")
                .With("Negative", leftBindString)
                .With("Positive", bindingString);
        }
        else if (currentBinderType == binderType.interact)
        {
            for (int i = 0; i < menuManager.playerInput.Player.Interact.bindings.Count; i++)
            {
                menuManager.playerInput.Player.Interact.ChangeBinding(i).Erase();
            }
            menuManager.playerInput.Player.Interact.AddCompositeBinding("Axis")
                .With("Positive", bindingString);
        }
        else if (currentBinderType == binderType.escape)
        {

            for (int i = 0; i < menuManager.playerInput.Player.Escape.bindings.Count; i++)
            {
                menuManager.playerInput.Player.Escape.ChangeBinding(i).Erase();
            }
            menuManager.playerInput.Player.Escape.AddCompositeBinding("Axis")
                .With("Positive", bindingString);
        }
        Debug.Log("Binding changed of " + bindingString);

        menuManager.playerInput.Enable();

    }

    public void GetAllBindingString()
    {
        allBindingStrings.Clear();
        int i = 0;
        foreach (InputBinding bind in menuManager.playerInput.Player.Move.bindings)
        {

            allBindingStrings.Add(menuManager.playerInput.Player.Move.bindings[i].GetNameOfComposite() + "COMPOSITE NAME Move");
            allBindingStrings.Add(bind.path);
            i++;
        }

        i = 0;
        foreach (InputBinding bind in menuManager.playerInput.Player.Interact.bindings)
        {
            allBindingStrings.Add(menuManager.playerInput.Player.Interact.bindings[i].GetNameOfComposite() + "COMPOSITE NAME Interact");
            allBindingStrings.Add(bind.path);
            i++;
        }

        i = 0;
        foreach (InputBinding bind in menuManager.playerInput.Player.Escape.bindings)
        {
            allBindingStrings.Add(menuManager.playerInput.Player.Escape.bindings[0].GetNameOfComposite() + "COMPOSITE NAME escape");
            allBindingStrings.Add(bind.path);
            i++;
        }
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
