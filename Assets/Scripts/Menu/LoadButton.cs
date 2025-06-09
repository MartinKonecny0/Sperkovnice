using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadButton : MenuElement
{
    public int saveSlot;
    private bool isEmptySlot;
    public Text loadInfoText;
    public Color highlightColor;

    void Start()
    {
        SetLoadInfo();
    }

    public void SetLoadInfo()
    {
        SaveManager.saveSlot = saveSlot;
        SceneData data = SaveManager.Load();
        if (data != null)
        {
            isEmptySlot = false;
            loadInfoText.text = data.metaData.saveDate + "\n" + data.playerData.character;
        }
        else
        {
            isEmptySlot = true;
            loadInfoText.text = "Empty load";
        }
    }

    public override void Interact()
    {
        Deselect();
        SaveManager.saveSlot = saveSlot;
        SceneManager.LoadScene(0);
    }

    public override void HoldInteract()
    {
        if (!isEmptySlot)
        {
            string path = Application.persistentDataPath + "/saveFile" + saveSlot + ".json";
            File.Delete(path);
            Debug.Log(Application.persistentDataPath + "/saveFile" + saveSlot + ".json" + " - DELETED");
            SetLoadInfo();
        }
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