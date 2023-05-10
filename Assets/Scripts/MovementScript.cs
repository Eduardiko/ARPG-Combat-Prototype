using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{

    //References
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    private CharacterController controller;
    private Animator playerAnimator;

    //Variables
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;
    private float moveSpeed = 0f;

    private Vector2 inputVector;
    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    private Vector3 moveDirection;

    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -9.81f;
    private Vector3 jumpVelocity;

    private float groundRayDistance = 0.4f;

    // State Bools
    private bool isRunning = false;
    private bool isGrounded = false;

    private bool tryingToRun = false;
    private bool tryingToJump = false;

    private bool ableToJump = false;
    private bool ableToRun = false;
    private bool movingInput = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<Animator>();
        jumpVelocity = Vector3.zero;
    }

    void Update()
    {
        SetPossibleActions();
        MoveLogic();
        JumpingLogic();
    }

    void SetPossibleActions()
    {
        // Check if the player is on the ground
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayDistance, groundMask);

        //Is There Input?
        if (inputVector != Vector2.zero)
            movingInput = true;
        else
            movingInput = false;

        // Can The Player Run?
        if (movingInput && isGrounded)
            ableToRun = true;
        else
            ableToRun = false;

        // Can The Player Jump?
        if (isGrounded && jumpVelocity.y < 0)
            ableToJump = true;
        else
            ableToJump = false;

    }

    #region MOVEMENT
    void MoveLogic()
    {
        //Set speeds and animation states
        if(movingInput)
        {
            if (ableToRun && tryingToRun)
                isRunning = true;
            else
                tryingToRun = false;

            if (isRunning && Vector2.Angle(currentInputVector, inputVector) < 90f)
                Run();
            else
                Walk();
        } else
        {
            Idle();
        }

        //Smooth Movement
        float smoothInputSpeed = 0.15f;
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputVector, ref smoothInputVelocity, smoothInputSpeed);
        moveDirection = new Vector3(currentInputVector.x, 0f, currentInputVector.y).normalized;

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
    void Idle()
    {
        // Manage Running
        if (isRunning)
            isRunning = false;

        tryingToRun = false;

        // Set Speed
        if (moveSpeed > 0.2f)
            moveSpeed = DecelerateSpeed(moveSpeed, 0.1f);
        else if (currentInputVector.magnitude < 0.01f)
        {
            currentInputVector = Vector2.zero;
            moveSpeed = 0f;
        }

        // Set Animation
        playerAnimator.SetTrigger("RunToIdle");
        playerAnimator.SetFloat("IdleToWalk", currentInputVector.magnitude / 3);

    }
    void Walk()
    {
        // Manage Running
        if (isRunning)
            isRunning = false;

        // Set Speed
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = walkSpeed * currentInputVector.magnitude;

        // Set Animation
        playerAnimator.SetTrigger("RunToIdle");
        playerAnimator.SetFloat("IdleToWalk", currentInputVector.magnitude/3);
    }

    void Run()
    {
        // Manage Running
        isRunning = true;
        
        // Set Speed
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = AccelerateSpeed(moveSpeed, 0.5f, runSpeed);
        if(moveSpeed > runSpeed) moveSpeed = runSpeed;


        // Set Animation
        playerAnimator.SetTrigger("WalkToRun");
    }
    #endregion

    #region JUMP
    void JumpingLogic()
    {
        if (ableToJump && tryingToJump)
            Jump();
        else
            tryingToJump = false;

        //Set jumpVelocity negative so we don't get errors with positive velocities
        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f;
        }

        //In-air logic
        if (jumpVelocity.y >= 0.3f)
        {
            jumpVelocity.y += (gravity - jumpVelocity.y) * Time.deltaTime;
        }
        else if(jumpVelocity.y < 0.3f && jumpVelocity.y >= 0f)
        {
            jumpVelocity.y += gravity/4f * Time.deltaTime;
        }
        else
        {
            jumpVelocity.y += (gravity - jumpVelocity.y * jumpVelocity.y / 5f) * 1.5f * Time.deltaTime;
        }
        
        // Move vertically
        controller.Move(jumpVelocity * Time.deltaTime);
    }
    void Jump()
    {
        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
        playerAnimator.SetTrigger("WalkToJump");
    }
    #endregion

    #region ACTIONS
    public void ActionMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
    public void ActionJump(InputAction.CallbackContext context)
    {
        if(context.performed) tryingToJump = true;
    }

    public void ActionRun(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToRun = true;
    }
    #endregion

    #region HERLPERS
    public float DecelerateSpeed(float currentSpeed, float decelerationTime)
    {
        float decelerationRate = Mathf.Log(2) / decelerationTime;
        float deceleratedSpeed = currentSpeed * Mathf.Exp(-decelerationRate * Time.deltaTime);
        return deceleratedSpeed;
    }

    public float AccelerateSpeed(float currentSpeed, float accelerationTime, float maxSpeed)
    {
        float accelerationRate = Mathf.Log(maxSpeed / currentSpeed) / accelerationTime;
        float acceleratedSpeed = currentSpeed * Mathf.Exp(accelerationRate * Time.deltaTime);
        return Mathf.Min(acceleratedSpeed, maxSpeed);
    }
    #endregion
}