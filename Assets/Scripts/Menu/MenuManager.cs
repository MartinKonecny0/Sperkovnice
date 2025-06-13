using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


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
    public Dictionary<int, string> currentBindDictionary;

    private void Awake()
    {
        playerInput = BindingManager.playerInput;
        BindingManager.InitBindingDictionary();
        LoadSettings();
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
        currentBindDictionary = new Dictionary<int, string>();
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        panelsStack = new Stack<GameObject>();
        selectedStack = new Stack<int>();
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
    public void SaveSettings()
    {
        SetupSave();
        SettingsSaveManager.Save();
    }
    public static void SetupSave()
    {
        SettingsSaveManager.allBindingStrings = BindingManager.currentBindDictionary;
        SettingsSaveManager.soundValue = BindingManager.soundValue;
    }
    public static void LoadSettings()
    {
        SettingsData loadedSettingsData = SettingsSaveManager.Load();
        if (loadedSettingsData == null)
        {
            Debug.Log("Custom settings file does not exist -> empty default settings");
            return;
        }

        foreach (var bind in loadedSettingsData.bindingData)
        {
            BindingManager.ChangeBinding(bind.binderType, bind.bindingString, true);
        }

    }
}