using UnityEngine;

public class DialogItem : InteractableObject
{
    // temporary -> in future prolly animation?
    public string[] allDialogs;
    private string currentDialog;
 
    void Start()
    {
        type = InteractableType.dialog;
        currentDialog = allDialogs[0];
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        ShowCurrentDialog();
    }

    public void ShowCurrentDialog()
    {
        print(currentDialog);
    }
}
