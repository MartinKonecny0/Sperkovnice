public class ForceDialog : ForcedAction
{
    public bool isActivatingDialog = true;
    public RoomManager roomManager;
    public Dialog dialogToForce;
    public override void ExecuteAction()
    {
        if (isActivatingDialog)
        {

        }
        else
        {
            Destroy(dialogToForce);

        }
    }
}
