using System.Collections.Generic;
using UnityEngine;

public enum CharacterType
{
    Engineer, // Topiè
    Hedwig, // Hedvika
    Priest, //Knìz
    Shaman, // Šaman
    Aunt, // Teta
}
public class CharacterItem : InteractableObject
{
    public CharacterType characterType;
    public List<Dialog> dialogsToSay;
    public bool isDialogEnabled = true;

    public bool isWalkingToDefault = false;
    public Transform defaultPosition;
    public float walkSpeed = 2;

    private float lastDistance;

    void Start()
    {
        type = InteractableType.character;
    }

    public override void Interact(GameObject oldCharacter, Player playerScript)
    {
        oldCharacter.GetComponent<CharacterItem>().StartDialogsWith(this.characterType);
        
        oldCharacter.GetComponent<CharacterItem>().StartWalkToDefault();
        playerScript.ChangePlayerToCharacter(oldCharacter, this.gameObject);
        // TODO: add executing forced action?
    }

    public void StartDialogsWith(CharacterType receiver)
    {
        List<Dialog> filteredDialogs = GetDialogsForCharacter(receiver);

        foreach (Dialog dialog in filteredDialogs)
        {
            Debug.Log("Character " + characterType + " saying: " + dialog.thingToSay);
        }
    }

    public List<Dialog> GetDialogsForCharacter(CharacterType receiverCharacterType)
    {
        List<Dialog> filteredDialogs = new List<Dialog>();
        foreach (Dialog dialog in dialogsToSay)
        {
            if (dialog == null)
            {
                continue;
            }

            if (dialog.receiver == receiverCharacterType)
            {
                filteredDialogs.Add(dialog);
            }
        }
        return filteredDialogs;
    }
    public void AddDialogToSay(Dialog dialogToSay)
    {
        dialogsToSay.Add(dialogToSay);
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
            float distance = defaultPosition.position.x - transform.position.x;
            if (distance * lastDistance < 0)
            {
                GetComponent<Rigidbody2D>().linearVelocityX = 0;
                isWalkingToDefault = false;
            }
            else
            {
                float horizontal;
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
