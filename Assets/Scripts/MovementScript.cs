using UnityEngine;
using UnityEngine.InputSystem;


// Strings To Re-Use and Not memorize all animation parameters
[System.Serializable]
public class AnimationTriggerKeys
{
    // Triggers
    public string jumpTriggerKey;
    public string runTriggerKey;

    // Bools
    public string isRunningKey;
    public string isLockingKey;

    // Floats
    public string directionXKey;
    public string directionZKey;
}

// Sekiro Lock -> front, normal speed || back and sides, reduced speeds

public class MovementScript : MonoBehaviour
{
    
    //References
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    private CharacterController characterController;
    private CameraController cameraController;
    private Animator characterAnimator;

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

    [SerializeField] AnimationTriggerKeys animKeys;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterAnimator = GetComponentInChildren<Animator>();
        cameraController = GetComponentInChildren<CameraController>();

        jumpVelocity = Vector3.zero;
    }

    private void Update()
    {
        UpdatePossibleActions();
        UpdateStatesAndAnimations();
        MoveLogic();
        JumpingLogic();
    }

    private void UpdatePossibleActions()
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

    private void UpdateStatesAndAnimations()
    {
        // Animation Mode
        if (cameraController.isLocking)
            characterAnimator.SetBool(animKeys.isLockingKey, true);
        else
            characterAnimator.SetBool(animKeys.isLockingKey, false);

        // Walking
        if (movingInput)
        {
            if (ableToRun && tryingToRun)
                EnterRunning();
            else
                tryingToRun = false;

            if (isRunning && Vector2.Angle(currentInputVector, inputVector) < 90f)
                Run();
            else
            {
                if (isRunning)
                    QuitRunning();

                Walk();
            }
        }
        else
        {
            if (isRunning)
                QuitRunning();

            Idle();
        }

        // Jumping
        if (ableToJump && tryingToJump)
            Jump();
        else
            tryingToJump = false;
    }

    #region MOVEMENT
    private void MoveLogic()
    {
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


        // Move the player
        if (moveDirection != Vector3.zero)
        {
            if(!cameraController.isLocking || isRunning)
            {
                // Rotate the player to face the direction of movement
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            } else
            {
                transform.rotation = Quaternion.LookRotation(cameraController.playerToNearestEnemyVector);
            }


            characterController.Move(moveDirection);
        }
    }
    private void Idle()
    {
        // Set Speed
        if (moveSpeed > 0.2f)
            moveSpeed = DecelerateSpeed(moveSpeed, 0.1f);
        else if (currentInputVector.magnitude < 0.01f)
        {
            currentInputVector = Vector2.zero;
            moveSpeed = 0f;
        }

        // Set Animation
        if(cameraController.isLocking)
        {
            characterAnimator.SetFloat(animKeys.directionXKey, currentInputVector.x);
            characterAnimator.SetFloat(animKeys.directionZKey, currentInputVector.y);
        } else
        {
            characterAnimator.SetFloat(animKeys.directionZKey, currentInputVector.magnitude);
        }

    }
    private void Walk()
    {
        // Set Speed
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = walkSpeed * currentInputVector.magnitude;

        // Set Animation
        if (cameraController.isLocking)
        {
            characterAnimator.SetFloat(animKeys.directionXKey, currentInputVector.x);
            characterAnimator.SetFloat(animKeys.directionZKey, currentInputVector.y);
        }
        else
        {
            characterAnimator.SetFloat(animKeys.directionZKey, currentInputVector.magnitude);
        }
    }

    private void Run()
    {       
        // Set Speed
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        moveSpeed = AccelerateSpeed(moveSpeed, 0.5f, runSpeed);
        if(moveSpeed > runSpeed) moveSpeed = runSpeed;
    }
    #endregion

    #region JUMP
    private void JumpingLogic()
    {
        //Set jumpVelocity negative so we don't get errors with positive velocities
        if (isGrounded && jumpVelocity.y < 0)
            jumpVelocity.y = -2f;

        //In-air logic
        if (jumpVelocity.y >= 0.3f)
            jumpVelocity.y += (gravity - jumpVelocity.y) * Time.deltaTime;
        else if(jumpVelocity.y < 0.3f && jumpVelocity.y >= 0f)
            jumpVelocity.y += gravity/4f * Time.deltaTime;
        else
            jumpVelocity.y += (gravity - jumpVelocity.y * jumpVelocity.y / 5f) * 1.5f * Time.deltaTime;
        
        // Move vertically
        characterController.Move(jumpVelocity * Time.deltaTime);
    }
    private void Jump()
    {
        jumpVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);

        // Set Animation
        characterAnimator.SetTrigger(animKeys.jumpTriggerKey);
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
    private float DecelerateSpeed(float currentSpeed, float decelerationTime)
    {
        float decelerationRate = Mathf.Log(2) / decelerationTime;
        float deceleratedSpeed = currentSpeed * Mathf.Exp(-decelerationRate * Time.deltaTime);
        return deceleratedSpeed;
    }

    private float AccelerateSpeed(float currentSpeed, float accelerationTime, float maxSpeed)
    {
        float accelerationRate = Mathf.Log(maxSpeed / currentSpeed) / accelerationTime;
        float acceleratedSpeed = currentSpeed * Mathf.Exp(accelerationRate * Time.deltaTime);
        return Mathf.Min(acceleratedSpeed, maxSpeed);
    }

    private void EnterRunning()
    {
        isRunning = true;
        tryingToRun = false;

        // Set Animation
        characterAnimator.SetBool(animKeys.isRunningKey, true);
    }
    private void QuitRunning()
    {
        isRunning = false;
        tryingToRun = false;

        // Set Animation
        characterAnimator.SetBool(animKeys.isRunningKey, false);
    }

    #endregion
}