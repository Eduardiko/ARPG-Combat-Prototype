using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{

    //References
    private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;
    private Animator playerAnimator;

    //Variables
    [SerializeField] private float moveSpeed = 0f;
    [SerializeField] private float walkSpeed = 5f;
    private Vector3 moveDirection;
    private Vector2 inputVector;

    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 velocity;

    private bool isGrounded;
    public LayerMask groundMask;
    public float groundRayDistance = 0.4f;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        velocity = Vector3.zero;
    }

    void Update()
    {
        MovePlayer();
        JumpingLogic();
    }


    public void SetDirection(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();

        // Get input from the Xbox controller
        moveDirection = new Vector3(inputVector.x, 0f, inputVector.y).normalized;

        // Get the camera's forward vector
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Calculate the direction to move the player and multiply it by the speed
        moveDirection = cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x;
        moveDirection *= moveSpeed * Time.deltaTime;

        // Rotate the player to face the direction of movement
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
    }

    void MovePlayer()
    {
        //Set speeds and animation states
        if (moveDirection != Vector3.zero)
            Walk();
        else if (moveDirection == Vector3.zero)
            Idle();

        // Move the player
        if (moveDirection != Vector3.zero) controller.Move(moveDirection);
    }

    void JumpingLogic()
    {
        //Set velocity negative so we don't get errors with positive velocities
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //In-air logic
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    void Idle()
    {
        playerAnimator.SetFloat("IdleToWalk", 0f);
    }

    void Walk()
    {
        float joystickInclination = inputVector.magnitude;
        playerAnimator.SetFloat("IdleToWalk", joystickInclination);

        moveSpeed = walkSpeed * joystickInclination;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // Check if the player is on the ground
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayDistance, groundMask);

        //Set velocity negative so we don't get errors with positive velocities
        if (isGrounded && velocity.y < 0 && context.performed)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        }
    }
}