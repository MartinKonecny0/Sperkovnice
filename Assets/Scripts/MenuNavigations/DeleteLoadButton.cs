using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeleteLoadButton : MenuElement
{
    public int saveSlot;
    public Color highlightColor;
    public override void Interact()
    {
        string path = Application.persistentDataPath + "/saveFile" + saveSlot + ".json";
        File.Delete(path);
        Debug.Log(Application.persistentDataPath + "/saveFile" + saveSlot + ".json" + " - DELETED");
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
