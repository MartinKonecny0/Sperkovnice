using UnityEngine;
using static BinderElement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public static class BindingManager
{
    public static float soundValue;
    public static Dictionary<int, string> currentBindDictionary = new Dictionary<int, string>();
    public static InputSystem_Actions playerInput = new InputSystem_Actions();

    public static void InitBindingDictionary()
    {
        string leftBindString = playerInput.Player.Move.bindings[1].path;
        UpdateDictionary(binderType.left, leftBindString);

        string rightBindString = playerInput.Player.Move.bindings[2].path;
        UpdateDictionary(binderType.right, rightBindString);


        string interactBindString = playerInput.Player.Interact.bindings[0].path;
        UpdateDictionary(binderType.interact, interactBindString);

        string escapeBindString = playerInput.Player.Escape.bindings[0].path;
        UpdateDictionary(binderType.escape, escapeBindString);
    }
    public static void ChangeBinding(binderType currentBinderType, string bindingString, bool ignoreCollisions)
    {
        if (currentBindDictionary.ContainsValue(bindingString) && !ignoreCollisions)
        {
            Debug.LogError("Cant bind one key to two different actions");
            return;
        }

        playerInput.Disable();

        if (currentBinderType == binderType.left)
        {
            string rightBindString = playerInput.Player.Move.bindings[2].path;
            for (int i = 0; i < playerInput.Player.Move.bindings.Count; i++)
            {
                playerInput.Player.Move.ChangeBinding(i).Erase();
            }
            playerInput.Player.Move.AddCompositeBinding("Axis")
                .With("Negative", bindingString)
                .With("Positive", rightBindString);
        }
        else if (currentBinderType == binderType.right)
        {
            string leftBindString = playerInput.Player.Move.bindings[1].path;
            for (int i = 0; i < playerInput.Player.Move.bindings.Count; i++)
            {
                playerInput.Player.Move.ChangeBinding(i).Erase();
            }
            playerInput.Player.Move.AddCompositeBinding("Axis")
                .With("Negative", leftBindString)
                .With("Positive", bindingString);
        }
        else if (currentBinderType == binderType.interact)
        {
            for (int i = 0; i < playerInput.Player.Interact.bindings.Count; i++)
            {
                playerInput.Player.Interact.ChangeBinding(i).Erase();
            }
            playerInput.Player.Interact.AddCompositeBinding("Axis")
                .With("Positive", bindingString);
        }
        else if (currentBinderType == binderType.escape)
        {

            for (int i = 0; i < playerInput.Player.Escape.bindings.Count; i++)
            {
                playerInput.Player.Escape.ChangeBinding(i).Erase();
            }
            playerInput.Player.Escape.AddCompositeBinding("Axis")
                .With("Positive", bindingString);
        }
        UpdateDictionary(currentBinderType, bindingString);

        Debug.Log("Binding of " + currentBinderType + " changed to " + bindingString);
        //PrintDictionary();
        playerInput.Enable();
    }
    public static void PrintDictionary()
    {
        for (int i = 0; i < 4; i++)
        {
            if (currentBindDictionary.ContainsKey(i))
            {

                Debug.Log(currentBindDictionary[i]);
            }
        }
    }
    public static void UpdateDictionary(binderType currentBinderType, string bindString)
    {
        if (currentBindDictionary.ContainsKey((int)currentBinderType))
        {
            currentBindDictionary[(int)currentBinderType] = bindString;
        }
        else
        {
            currentBindDictionary.Add((int)currentBinderType, bindString);

        }
    }
}
