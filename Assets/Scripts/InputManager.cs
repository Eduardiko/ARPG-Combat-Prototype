using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Movement Inputs
    [HideInInspector] public bool tryingToRun = false;
    [HideInInspector] public bool tryingToJump = false;
    [HideInInspector] public bool tryingToMove = false;
    [HideInInspector] public Vector2 inputMoveVector = new Vector2();

    // Camera Inputs
    [HideInInspector] public bool tryingToLock = false;
    [HideInInspector] public bool tryingToLook = false;
    [HideInInspector] public Vector2 inputLookVector = new Vector2();

    // Offensive Inputs
    [HideInInspector] public bool tryingToAttack = false;


    void Start()
    {
        
    }

    void Update()
    {
        if (inputMoveVector != Vector2.zero)
            tryingToMove = true;
        else
            tryingToMove= false;

        if (inputLookVector != Vector2.zero)
            tryingToLook = true;
        else
            tryingToLook = false;
    }

    #region ACTIONS

    // Movement Actions
    public void ActionJump(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToJump = true;
    }

    public void ActionRun(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToRun = true;
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
        if (context.performed) tryingToAttack = true;
    }


    #endregion
}
