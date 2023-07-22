using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public enum BufferActions
{
    ACTION_RUN,
    ACTION_JUMP,
    ACTION_ATTACK,
    ACTION_BACKSTEP,
    ACTION_DODGE_RIGHT,
    ACTION_DODGE_LEFT,
    CLEAR
}

public class InputManager : MonoBehaviour
{
    // Movement Inputs
    [HideInInspector] public bool tryingToRun = false;                  
    [HideInInspector] public bool tryingToJump = false;                
    [HideInInspector] public bool tryingToMove = false;                 
    [HideInInspector] public bool tryingToBackstep = false;
    [HideInInspector] public bool tryingToDodgeRight = false;
    [HideInInspector] public bool tryingToDodgeLeft = false;
    [HideInInspector] public Vector2 inputMoveVector = new Vector2();

    // Camera Inputs
    [HideInInspector] public bool tryingToLock = false;
    [HideInInspector] public bool tryingToLook = false;
    [HideInInspector] public Vector2 inputLookVector = new Vector2();

    // Offensive Inputs
    [HideInInspector] public bool tryingToAttack = false;
    [HideInInspector] public Vector2 inputWeaponDialVector = new Vector2();

    // Input Manager Variables
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
                    case BufferActions.ACTION_BACKSTEP:
                        tryingToBackstep = true;
                        break;
                    case BufferActions.ACTION_DODGE_RIGHT:
                        tryingToDodgeRight = true;
                        break;
                    case BufferActions.ACTION_DODGE_LEFT:
                        tryingToDodgeLeft = true;
                        break;
                    default:
                        break;
                }
            }

            bufferedTimeRemaining -= Time.deltaTime;
        } else
        {

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

    public void ActionBackstep(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            tryingToBackstep = true;
            SendActionToInputBuffer(BufferActions.ACTION_BACKSTEP);
        }
    }
    public void ActionDodgeRight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tryingToDodgeRight = true;
            SendActionToInputBuffer(BufferActions.ACTION_DODGE_RIGHT);
        }
    }

    public void ActionDodgeLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tryingToDodgeLeft = true;
            SendActionToInputBuffer(BufferActions.ACTION_DODGE_LEFT);
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

    public void ActionAdjustWeaponDial(InputAction.CallbackContext context)
    {
        inputWeaponDialVector = context.ReadValue<Vector2>();
    }

    #endregion
}
