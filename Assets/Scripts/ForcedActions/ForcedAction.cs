using UnityEngine;

public abstract class ForcedAction : MonoBehaviour
{
    public enum actionType
    {
        CreateTask,
        CreateDialog,
        RemoveDialog,
        Animation,
    }

    private actionType typeOfAction;

    public abstract void ExecuteAction();

}
