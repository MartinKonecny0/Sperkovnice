using UnityEngine;

public enum CharacterType
{
    Hedwig, // Hedvika
    Engineer, // Topiè
    Shaman, // Šaman
}
public class CharacterItem : InteractableObject
{
    public CharacterType characterType;
    public string[] allDialogs;
    public bool isDialogEnabled = true;
    public int currDialogIndex;

    public bool isWalkingToDefault = false;
    public Transform defaultPosition;
    public float walkSpeed = 2;

    void Start()
    {
        type = InteractableType.character;
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        if (isDialogEnabled)
        {
            if (currDialogIndex <= allDialogs.Length - 1)
            {
                Debug.Log("Character " + characterType + " saying: " + allDialogs[currDialogIndex]);
            }
            else
            {
                Debug.LogError("Trying to start missing dialog");
            }
        }
        playerScript.ChangePlayerToCharacter(character, this.gameObject);
    }

    public void SetNextDialog()
    {
        isDialogEnabled = true;
        currDialogIndex++;
    }

    public void DisableDialog()
    {
        isDialogEnabled = false;
    }

    void Update()
    {
        if (isWalkingToDefault)
        {
            float deltaDefault = 0.1f;
            float horizontal;

            float distance = defaultPosition.position.x - transform.position.x;

            if (Mathf.Abs(distance) >= deltaDefault)
            {
                if (defaultPosition.position.x - transform.position.x < 0)
                {

                    horizontal = -1;
                }
                else
                {
                    horizontal = 1;
                }
                GetComponent<Rigidbody2D>().linearVelocityX = horizontal * walkSpeed;
            }
            else
            {
                GetComponent<Rigidbody2D>().linearVelocityX = 0;
                isWalkingToDefault = false;
            }
        }
    }
}
