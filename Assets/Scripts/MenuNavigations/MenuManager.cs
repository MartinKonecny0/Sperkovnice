using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MenuManager : MonoBehaviour
{
    public InputSystem_Actions playerInput;
    private InputAction movement;
    private InputAction interaction;

    private float lastHorizontal;
    private float horizontal;
    private float lastInteract;
    private float interact;

    public int selectedButtonIndex;
    public MenuElement[] allSelectableButtons;

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
    }
    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        GetActiveMenuElements();
        SelectButtonByIndex(selectedButtonIndex);
    }
    void Update()
    {
        horizontal = movement.ReadValue<float>();
        interact = interaction.ReadValue<float>();

        if (horizontal > 0 && lastHorizontal == 0)
        {
            SelectNextButton();
        }
        else if (horizontal < 0 && lastHorizontal == 0)
        {
            SelectPreviousButton();
        }

        if (interact > 0 && lastInteract == 0)
        {
            allSelectableButtons[selectedButtonIndex].Interact();
        }

        lastHorizontal = horizontal;
        lastInteract = interact;
    }

    public void PanelChanged()
    {
        GetActiveMenuElements();
        selectedButtonIndex = 0;
        SelectButtonByIndex(selectedButtonIndex);
    }
    private void GetActiveMenuElements()
    {
        allSelectableButtons = GetComponentsInChildren<MenuElement>();
    }

    private void SelectButtonByIndex(int indexToSelect)
    { 
        if (selectedButtonIndex < allSelectableButtons.Length)
        {
            allSelectableButtons[selectedButtonIndex].Deselect();
        }
        selectedButtonIndex = indexToSelect;
        if (indexToSelect < allSelectableButtons.Length)
        {
            allSelectableButtons[indexToSelect].Select();
        }
        else
        {
            Debug.LogError("Element with this index does not exists.");
        }
    }

    private void SelectNextButton()
    {
        allSelectableButtons[selectedButtonIndex].Deselect();
        selectedButtonIndex = (selectedButtonIndex + 1) % allSelectableButtons.Length;
        allSelectableButtons[selectedButtonIndex].Select();
    }
    private void SelectPreviousButton()
    {
        allSelectableButtons[selectedButtonIndex].Deselect();
        selectedButtonIndex = (selectedButtonIndex - 1) % allSelectableButtons.Length;
        if (selectedButtonIndex < 0)
        {
            selectedButtonIndex = allSelectableButtons.Length - 1;
        }
        allSelectableButtons[selectedButtonIndex].Select();
    }
}