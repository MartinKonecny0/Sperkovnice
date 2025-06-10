using UnityEngine;

public class ForceDialog : ForcedAction
{
    public bool isActivatingDialog = true;
    public RoomManager roomManager;
    public Dialog dialogToForce;
    public override void ExecuteAction()
    {
        if (isActivatingDialog)
        {
            CharacterItem speakerItem = roomManager.GetCharacterInstanceByType(dialogToForce.speaker);
            speakerItem.AddDialogToSay(dialogToForce);
        }
        else
        {
            Destroy(dialogToForce);

        }
    }
}
