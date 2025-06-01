using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadButton : MenuElement
{
    public int saveSlot;
    public Color highlightColor;
    public override void Interact()
    {
        Deselect();
        SaveManager.saveSlot = saveSlot;
        SceneManager.LoadScene(0);
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
