using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputSystem_Actions playerInput;
    private InputAction movement;

    public float horizontal = 0f;
    public float speed = 0.25f;
    public GameObject player;

    private RaycastHit2D hit;

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
    }

    // na zacatku
    void Start()
    {
        player = this.gameObject;
    }

    private void OnEnable()
    {
        movement = playerInput.Player.Move;
        movement.Enable();
        
        playerInput.Player.Interact.performed += Interact;
        playerInput.Player.Interact.Enable();
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (hit)
        {

            Destroy(hit.collider.gameObject);
        }

    }


    // every frame
    private void Update()
    {
        horizontal = movement.ReadValue<float>();
        player.GetComponent<Rigidbody2D>().linearVelocityX = horizontal * speed;
        LayerMask layer = LayerMask.GetMask("Interactable");
        // Cast a ray straight down.
        hit = Physics2D.Raycast(transform.position, Vector2.up, Mathf.Infinity, layer);

        if (hit)
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
            Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
