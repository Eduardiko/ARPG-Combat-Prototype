using UnityEngine;

public class MovementScript : MonoBehaviour
{
    //References
    private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;

    //Variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 5f;

    [SerializeField] private float turnSpeed = 180f;

    private Vector3 moveDirection;

    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    public LayerMask groundMask;
    public float groundRayDistance = 0.4f;
    private bool isGrounded;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        velocity = Vector3.zero;
    }

    void Update()
    {
        //Allow the player to jump
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Check if the player is on the ground
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayDistance, groundMask);
        if (!isGrounded)
        {
            // Move the player using the character controller
            controller.Move(velocity);

            velocity.y -= 5f * Time.deltaTime;
        }

        Move();
    }

    void Move()
    {
        // Get input from the Xbox controller
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");
        moveDirection = new Vector3(xMovement, 0f, zMovement).normalized;

        // Get the camera's forward vector
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Calculate the direction to move the player and multiply it by the speed
        moveDirection = cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x;
        moveDirection *= moveSpeed * Time.deltaTime;

        // Move the player
        if(moveDirection != Vector3.zero)
        {
            //ToDo: run/walk------------

            controller.Move(moveDirection);

            // Rotate the player to face the direction of movement
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }

    void Jump()
    {
        // Allow the player to jump
        if (isGrounded)
        {
            velocity.y = jumpHeight * gravity * -0.005f;

            // Move the player using the character controller
            controller.Move(velocity);
        }
    }
}