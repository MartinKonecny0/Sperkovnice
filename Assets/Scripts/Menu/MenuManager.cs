using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BinderElement;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MenuManager : MonoBehaviour
{
    public InputSystem_Actions playerInput;
    private InputAction movement;
    private InputAction interaction;
    private InputAction escape;

    private float lastHorizontal;
    private float horizontal;
    private float lastInteract;
    private float interact;
    private float lastBack;
    private float back;

    public bool skipNextButtonUp;
    private float holdSelectionTimer;

    public MenuElement[] currSelectElements;

    public Stack<int> selectedStack;
    public GameObject initialPanel;
    public Stack<GameObject> panelsStack;

    // settings save variables
    public Dictionary<int, string> allBindingStrings;
    public float soundValue;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
    }
    private void OnEnable()
    {
        movement = playerInput.Player.Move;
        movement.Enable();

        interaction = playerInput.Player.Interact;
        interaction.Enable();

        escape = playerInput.Player.Escape;
        escape.Enable();
    }
    void Start()
    {
        allBindingStrings = new Dictionary<int, string>();
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        panelsStack = new Stack<GameObject>();
        selectedStack = new Stack<int>();
        LoadSettings();
        AddPanel(initialPanel);
        GetActiveMenuElements();
        SelectButtonByIndex(selectedStack.First());
    }
    void Update()
    {
        horizontal = movement.ReadValue<float>();
        interact = interaction.ReadValue<float>();
        back = escape.ReadValue<float>();
        MenuElement selectedElement = currSelectElements[selectedStack.First()];

        // right button up
        if (horizontal == 0 && lastHorizontal > 0)
        {
            if (!skipNextButtonUp)
            {
                selectedElement.Right();
            }
            HoldReset();
        }
        // right button hold
        else if (horizontal > 0)
        {
            holdSelectionTimer += Time.deltaTime;
            if (holdSelectionTimer >= selectedElement.interactHoldCooldown)
            {
                selectedElement.RightHold();
                skipNextButtonUp = true;
            }
        }

        // left button up
        if (horizontal == 0 && lastHorizontal < 0)
        {
            if (!skipNextButtonUp)
            {
                selectedElement.Left();
            }
            HoldReset();
        }
        // left button hold
        else if (horizontal < 0)
        {
            holdSelectionTimer += Time.deltaTime;
            if (holdSelectionTimer >= selectedElement.interactHoldCooldown)
            {
                selectedElement.LeftHold();
                skipNextButtonUp = true;
            }
        }

        // interact button up
        if (interact == 0 && lastInteract > 0)
        {
            if (!skipNextButtonUp)
            {
                selectedElement.Interact();
            }
            HoldReset();
        }
        // interact button hold
        if (interact > 0)
        {
            //TODO: add some visual animation
            holdSelectionTimer += Time.deltaTime;
            if (holdSelectionTimer >= selectedElement.interactHoldCooldown)
            {
                selectedElement.HoldInteract();
                skipNextButtonUp = true;
            }
        }

        // escape button up
        if (back == 0 && lastBack > 0)
        {
            if (!skipNextButtonUp)
            {
                RemovePanel();
            }
            HoldReset();
        }

        lastHorizontal = horizontal;
        lastInteract = interact;
        lastBack = back;
    }
    private void HoldReset()
    {
        skipNextButtonUp = false;
        holdSelectionTimer = 0;
    }
    public void AddPanel(GameObject panelToAdd)
    {
        panelToAdd.SetActive(true);
        panelsStack.Push(panelToAdd);

        selectedStack.Push(0);
        GetActiveMenuElements();

        SelectButtonByIndex(0);
    }
    public void RemovePanel()
    {
        if (panelsStack.Count > 1)
        {
            currSelectElements[selectedStack.First()].Deselect();
            selectedStack.Pop();

            panelsStack.First().SetActive(false);
            panelsStack.Pop();

            GetActiveMenuElements();
            SelectButtonByIndex(selectedStack.First());
        }
    }
    private void GetActiveMenuElements()
    {
        GameObject currentPanel = panelsStack.First();
        currSelectElements = currentPanel.GetComponentsInChildren<MenuElement>();
    }
    private void SelectButtonByIndex(int indexToSelect)
    {
        if (selectedStack.First() < currSelectElements.Length)
        {
            currSelectElements[selectedStack.First()].Deselect();
        }

        selectedStack.Pop();
        selectedStack.Push(indexToSelect);
        if (indexToSelect < currSelectElements.Length)
        {
            currSelectElements[indexToSelect].Select();
        }
        else
        {
            Debug.LogError("Element with this index does not exists.");
        }
    }
    public void SelectNextButton()
    {

        currSelectElements[selectedStack.First()].Deselect();
        int currSelected = (selectedStack.First() + 1) % currSelectElements.Length;
        selectedStack.Pop();
        selectedStack.Push(currSelected);

        // skips disabled buttons
        if (!currSelectElements[currSelected].isActiveAndEnabled)
        {
            SelectNextButton();
        }

        currSelectElements[currSelected].Select();
    }
    public void SelectPreviousButton()
    {
        currSelectElements[selectedStack.First()].Deselect();
        int currSelected = (selectedStack.First() - 1) % currSelectElements.Length;
        selectedStack.Pop();

        if (currSelected < 0)
        {
            currSelected = currSelectElements.Length - 1;
        }
        selectedStack.Push(currSelected);

        // skips disabled buttons
        if (!currSelectElements[currSelected].isActiveAndEnabled)
        {
            SelectPreviousButton();
        }
        currSelectElements[currSelected].Select();
    }

    public void ChangeBinding(binderType currentBinderType, string bindingString)
    {
        playerInput.Disable();
        if (allBindingStrings.ContainsValue(bindingString))
        {
            Debug.LogError("Cant bind one key to two different actions");
            playerInput.Enable();

            return;
        }

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
        ChangeBindSettings(currentBinderType, bindingString);

        Debug.Log("Binding of " + currentBinderType + " changed to " + bindingString);
        playerInput.Enable();
    }

    public void SaveSettings()
    {
        SetupSave();
        SettingsSaveManager.Save();
    }
    public void SetupSave()
    {
        SettingsSaveManager.allBindingStrings = allBindingStrings;
        SettingsSaveManager.soundValue = soundValue;
    }

    public void ChangeBindSettings(binderType currentBinderType,string bindString)
    {
        if (allBindingStrings.ContainsKey((int)currentBinderType))
        {
            allBindingStrings[(int)currentBinderType] = bindString;
        }
        else
        {
            allBindingStrings.Add((int)currentBinderType, bindString);

        }
    }

    public void LoadSettings()
    {
        SettingsData loadedSettingsData = SettingsSaveManager.Load();
        if (loadedSettingsData == null)
        {
            Debug.Log("Custom settings file does not exist -> empty default settings");
            return;
        }

        foreach (var bind in loadedSettingsData.bindingData)
        {
            ChangeBinding(bind.binderType, bind.bindingString);
        }

    }
}