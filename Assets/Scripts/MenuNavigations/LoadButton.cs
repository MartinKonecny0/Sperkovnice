using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadButton : MenuElement
{
    public int saveSlot;
    private DeleteLoadButton loadButton;
    public Text loadInfoText;
    public Color highlightColor;

    void Start()
    {
        loadButton = GetComponentInChildren<DeleteLoadButton>();
        SetLoadInfo();
    }

    public void SetLoadInfo()
    {
        SaveManager.saveSlot = saveSlot;
        SceneData data = SaveManager.Load();
        if (data != null)
        {
            loadButton.Enable();
            loadInfoText.text = data.metaData.saveDate + "\n" + data.playerData.character;
        }
        else
        {
            loadButton.Disable();
            loadInfoText.text = "Empty load";
        }
    }

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
