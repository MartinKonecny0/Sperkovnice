using UnityEngine;

public class ForceEnableObjects : ForcedAction
{
    public GameObject[] objectsToActivate; //intentional usage for tasks and pickup items
                                        //
                                        //mby for puzzles?
    public override void ExecuteAction()
    {
        foreach (GameObject objectToEnable in objectsToActivate)
        {
            objectToEnable.SetActive(true);
        }
    }
}
