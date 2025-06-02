using UnityEditor.Rendering;
using UnityEngine;

public class PuzzleItem : InteractableObject
{
    // variable of puzzle screen
    public void Start()
    {
        type = InteractableType.puzzle;
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        Debug.Log("Interacted with puzzle -> show puzzle screen");
    }
}
