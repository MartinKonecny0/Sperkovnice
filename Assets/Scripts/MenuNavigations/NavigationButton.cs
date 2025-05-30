using UnityEngine;

public class NavigationButton : MonoBehaviour
{
    public GameObject panelToEnable;
    public GameObject panelToDisable;

    public void OnClick()
    {
        panelToEnable.SetActive(true);
        panelToDisable.SetActive(false);
    }
}
