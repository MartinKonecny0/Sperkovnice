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

    public bool isWalkingToDefault = false;
    public Transform defaultPosition;
    public float walkSpeed = 2;

    void Start()
    {
        type = InteractableType.character;
    }

    public override void Interact(GameObject character, Player playerScript)
    {
        playerScript.ChangePlayerToCharacter(character, this.gameObject);
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
