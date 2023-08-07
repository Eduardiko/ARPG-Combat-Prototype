using UnityEngine;


public class MovementScript : MonoBehaviour
{
    [Header("Move Parameters")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 8f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    
    // References
    private Character character;
    private CharacterController characterController;
    private InputManager inputManager;
    private Animator characterAnimator;

    // Variables
    private float moveSpeed = 0f;
    private float groundRayDistance = 0.4f;

    private Vector2 currentInputVector;
    private Vector2 smoothInputVelocity;

    private Vector3 moveDirection;
    private Vector3 jumpVelocity;

    // Bools
    private bool ableToJump = false;
    private bool ableToRun = false;


    private void Start()
    {
        character = GetComponentInChildren<Character>();
        characterController = GetComponent<CharacterController>();
        inputManager = GetComponentInChildren<InputManager>();
        characterAnimator = GetComponentInChildren<Animator>();

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
        character.isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundRayDistance, groundMask);

        // Can The Player Run?
        if (character.isGrounded)
            ableToRun = true;
        else
            ableToRun = false;

        // Can The Player Jump?
        if (character.isGrounded && jumpVelocity.y < 0 && !character.isPerformingAnAction && !character.isLocking)
            ableToJump = true;
        else
            ableToJump = false;

    }

    private void UpdateStatesAndAnimations()
    {
        // Animation Mode
        if (character.isLocking)
            characterAnimator.SetBool(character.animKeys.isLockingKey, true);
        else
            characterAnimator.SetBool(character.animKeys.isLockingKey, false);

        // Walking
        if (inputManager.tryingToMove && !character.isMovementRestriced)
        {
            if (ableToRun && inputManager.tryingToRun)
                EnterRunning();
            else
                inputManager.tryingToRun = false;

            if (character.isRunning)
                Run();
            else
            {
                if (character.isRunning)
                    QuitRunning();

                Walk();
            }
        }
        else
        {
            if (character.isRunning)
                QuitRunning();

            Idle();
        }

        // Jumping
        if (ableToJump && inputManager.tryingToJump)
            Jump();
        else 
            inputManager.tryingToJump = false;
    }

    #region MOVEMENT
    private void MoveLogic()
    {
        //Smooth Movement
        float smoothInputSpeed = 0.15f;
        currentInputVector = Vector2.SmoothDamp(currentInputVector, inputManager.inputMoveVector, ref smoothInputVelocity, smoothInputSpeed);
        moveDirection = new Vector3(currentInputVector.x, 0f, currentInputVector.y).normalized;

        // Get the camera's forward vector
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        // Calculate the direction to move the player and multiply it by the speed
        moveDirection = cameraForward * moveDirection.z + cameraTransform.right * moveDirection.x;
        moveDirection *= moveSpeed * Time.deltaTime;


        // Move and rotate the player
        if (moveDirection != Vector3.zero)
        {
            if(!character.isLocking || character.isRunning || !character.isGrounded)
            {
                // Rotate the player to face the direction of movement
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
            } else
            {
                // Y axis to 0 so Vector is calculated at same height
                Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
                Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
                transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
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
        characterAnimator.SetFloat(character.animKeys.directionXKey, currentInputVector.x);
        characterAnimator.SetFloat(character.animKeys.directionZKey, currentInputVector.y);

    }
    private void Walk()
    {
        // Set Speed
        if (currentInputVector.magnitude < 0.01f) currentInputVector = Vector2.zero;
        
        if(!character.isLocking)
            moveSpeed = walkSpeed * currentInputVector.magnitude;
        else 
        {
            Vector2 reducedVec = currentInputVector;

            reducedVec.x = reducedVec.x * 0.75f;
            if(currentInputVector.y < 0f) reducedVec.y = reducedVec.y * 0.8f;

            moveSpeed = walkSpeed * reducedVec.magnitude;
        }

        // Set Animation
        if (character.isLocking)
        {
            characterAnimator.SetFloat(character.animKeys.directionXKey, currentInputVector.x);
            characterAnimator.SetFloat(character.animKeys.directionZKey, currentInputVector.y);
        }
        else
        {
            characterAnimator.SetFloat(character.animKeys.directionZKey, currentInputVector.magnitude);
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
        if (character.isGrounded && jumpVelocity.y < 0)
            jumpVelocity.y = -2f;

        //In-air logic
        if (jumpVelocity.y >= 0.3f)
        {
            character.isGrounded = false;
            jumpVelocity.y += (gravity - jumpVelocity.y) * Time.deltaTime;
        }
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
        characterAnimator.SetTrigger(character.animKeys.jumpTriggerKey);

        inputManager.bufferedAction = BufferActions.CLEAR;
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
        character.isRunning = true;
        inputManager.tryingToRun = false;
        inputManager.bufferedAction = BufferActions.CLEAR;

        // Set Animation
        characterAnimator.SetBool(character.animKeys.isRunningKey, true);
    }
    private void QuitRunning()
    {
        character.isRunning = false;
        inputManager.tryingToRun = false;

        // Set Animation
        characterAnimator.SetBool(character.animKeys.isRunningKey, false);
    }

    #endregion
}