using UnityEngine;
using UnityEngine.UI;

public class NavigationButton : MenuElement
{
    public Color highlightColor;
    public GameObject panelToAdd;
    private MenuManager menuManager;
    void Start()
    {
        menuManager = GameObject.Find("Canvas").GetComponent<MenuManager>();
    }
    public override void Interact()
    {
        menuManager.AddPanel(panelToAdd);
    }
    public override void Select()
    {
        GetComponent<Image>().color = highlightColor;
    }
    public override void Deselect()
    {
        GetComponent<Image>().color = Color.white;
    }
}
