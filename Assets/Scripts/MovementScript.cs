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
    [SerializeField] private float runSpeed = 8f;
    private bool isRunning = false;
    private Vector3 moveDirection;
    private Vector2 inputVector;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

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
        MoveLogic();
        JumpingLogic();
    }


    void MoveLogic()
    {

        //Smooth Movement
        float smoothInputSpeed = 0.15f;
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        moveDirection = new Vector3(currentInputVector.x, 0f, currentInputVector.y).normalized;

        //Set speeds and animation states
        if (moveDirection != Vector3.zero && !isRunning)
            SetWalk();
        else if (moveDirection != Vector3.zero && isRunning)
            SetRun();
        else if (moveDirection == Vector3.zero)
            SetIdle();

        //Quit Running
        if (moveSpeed <= 4f || Vector2.Angle(currentInputVector, inputVector) > 90f)
        {
            playerAnimator.SetTrigger("RunToIdle");
            isRunning = false;
        }

        // Get the camera's forward vector
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Calculate the direction to move the player and multiply it by the speed
        moveDirection = cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x;
        moveDirection *= moveSpeed * Time.deltaTime;

        // Rotate the player to face the direction of movement
        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        // Move the player
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            controller.Move(moveDirection);
        }
    }

    void JumpingLogic()
    {
        // Check if the player is on the ground
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayDistance, groundMask);

        //Set velocity negative so we don't get errors with positive velocities
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        //In-air logic

        if (velocity.y >= 0.3f)
        {
            velocity.y += (gravity - velocity.y) * Time.deltaTime;
        }
        else if(velocity.y < 0.3f && velocity.y >= 0f)
        {
            velocity.y += gravity/5f * Time.deltaTime;
        }
        else
        {
            velocity.y += (gravity - velocity.y * velocity.y / 5f) * 1.75f * Time.deltaTime;
        }
 

        controller.Move(velocity * Time.deltaTime);
    }

    void SetIdle()
    {
        playerAnimator.SetFloat("IdleToWalk", 0f);
        isRunning = false;
    }
    void SetWalk()
    {
        playerAnimator.SetFloat("IdleToWalk", currentInputVector.magnitude/3);
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = walkSpeed * currentInputVector.magnitude;
    }

    void SetRun()
    {
        playerAnimator.SetTrigger("WalkToRun");

        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = runSpeed * currentInputVector.magnitude;
    }

    void SetJump()
    {
        //ToDo: If Animator is idle "IdleToJump" if not "WalkToJump"
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        playerAnimator.SetTrigger("WalkToJump");
    }

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
    public void ActionJump(InputAction.CallbackContext context)
    {
        if (isGrounded && velocity.y < 0 && context.performed)
        {
            SetJump();
        }
    }

    public void ActionRun(InputAction.CallbackContext context)
    {
        if(context.performed && isGrounded) isRunning = true;
    }
}