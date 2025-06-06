using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteLoadButton : MenuElement
{
    private LoadButton parentLoadButton;
    private MenuManager menuManager;
    private int saveSlot;
    public Color highlightColor;

    void Start()
    {
        parentLoadButton = transform.parent.GetComponent<LoadButton>();
        saveSlot = parentLoadButton.saveSlot;
    }

    public override void Interact()
    {
        string path = Application.persistentDataPath + "/saveFile" + saveSlot + ".json";
        File.Delete(path);
        Debug.Log(Application.persistentDataPath + "/saveFile" + saveSlot + ".json" + " - DELETED");
        parentLoadButton.SetLoadInfo();
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
