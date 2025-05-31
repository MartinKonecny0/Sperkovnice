using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject selectedElement;

    public GameObject[] allSelectableElements;
    public void StartMainScene(int saveSlot)
    {
        SaveManager.saveSlot = saveSlot;

        SceneManager.LoadScene(0);
    }
}