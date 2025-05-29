using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void SwapPanels(GameObject panelToDisable, GameObject panelToEnable)
    {
        panelToDisable.SetActive(false);
        panelToEnable.SetActive(false);
    }

    public void StartMainScene(int saveSlot)
    {
        SaveManager.saveSlot = saveSlot;

        SceneManager.LoadScene(0);
    }
}
