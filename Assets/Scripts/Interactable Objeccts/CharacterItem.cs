using UnityEngine;
using UnityEngine.TextCore.Text;

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

    private float lastDistance;

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

        character.GetComponent<CharacterItem>().StartWalkToDefault();
        
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

    public void StartWalkToDefault()
    {
        lastDistance = 0;
        defaultPosition = transform.parent.GetComponent<Room>().defaultPosition.transform;
        isWalkingToDefault = true;
    }

    void Update()
    {
        if (isWalkingToDefault)
        {
            float deltaDefault = 0.05f;
            float horizontal;

            float distance = defaultPosition.position.x - transform.position.x;
            if (distance * lastDistance < 0)
            {
                GetComponent<Rigidbody2D>().linearVelocityX = 0;
                isWalkingToDefault = false;
            }
            else
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

            lastDistance = distance;
        }
        //animator.speed = GetComponent<Rigidbody2D>().linearVelocityX;
    }
}
