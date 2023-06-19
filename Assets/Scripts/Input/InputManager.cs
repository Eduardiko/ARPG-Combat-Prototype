using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum BufferActions
{
    ACTION_RUN,
    ACTION_JUMP,
    ACTION_ATTACK,
    CLEAR,
}

public class InputManager : MonoBehaviour
{
    // Movement Inputs
    [HideInInspector] public bool tryingToRun = false;                  // Action 0
    [HideInInspector] public bool tryingToJump = false;                 // Action 1
    [HideInInspector] public bool tryingToMove = false;                 
    [HideInInspector] public Vector2 inputMoveVector = new Vector2();

    // Camera Inputs
    [HideInInspector] public bool tryingToLock = false;
    [HideInInspector] public bool tryingToLook = false;
    [HideInInspector] public Vector2 inputLookVector = new Vector2();

    // Offensive Inputs
    [HideInInspector] public bool tryingToAttack = false;               // Action 2


    [HideInInspector] public BufferActions bufferedAction;
    private float bufferTime = 0.3f;
    private float bufferedTimeRemaining = 0f;
    private bool isBuffering = false; 


    void Update()
    {
        UpdateInputBuffer();

        if (inputMoveVector != Vector2.zero)
            tryingToMove = true;
        else
            tryingToMove= false;

        if (inputLookVector != Vector2.zero)
            tryingToLook = true;
        else
            tryingToLook = false;
    }


    #region INPUT BUFFER

    public void SendActionToInputBuffer(BufferActions actionToBuffer)
    {
        isBuffering = true;
        bufferedAction = actionToBuffer;
        bufferedTimeRemaining = bufferTime;
    }

    private void UpdateInputBuffer()
    {
        if (isBuffering)
        {
            if (bufferedTimeRemaining <= 0)
            {
                isBuffering = false;

                switch (bufferedAction)
                {
                    case BufferActions.ACTION_RUN:
                        tryingToRun = true;
                        break;
                    case BufferActions.ACTION_JUMP:
                        tryingToJump = true;
                        break;
                    case BufferActions.ACTION_ATTACK:
                        tryingToAttack = true;
                        break;
                    default:
                        break;
                }
            }

            bufferedTimeRemaining -= Time.deltaTime;
        }
    }

    #endregion


    #region ACTIONS

    // Movement Actions
    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tryingToJump = true;
            SendActionToInputBuffer(BufferActions.ACTION_JUMP);
        }
    }

    public void ActionRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tryingToRun = true;
            SendActionToInputBuffer(BufferActions.ACTION_RUN);
        }
    }

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputMoveVector = context.ReadValue<Vector2>();
    }
    // Camera Actions
    public void ActionLook(InputAction.CallbackContext context)
    {
        inputLookVector = context.ReadValue<Vector2>();
    }

    public void ActionLock(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToLock = true;
    }

    // Offensive Actions
    public void ActionAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tryingToAttack = true;
            SendActionToInputBuffer(BufferActions.ACTION_ATTACK);
        }
    }


    #endregion
}
