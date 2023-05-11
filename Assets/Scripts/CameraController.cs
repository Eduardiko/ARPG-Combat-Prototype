using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeCamera;
    private Vector2 inputMoveVector;
    private Vector2 inputLookVector;

    // Update is called once per frame
    void Update()
    {
        //Recenter Camera If Walking Forwards
        if(inputLookVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeCamera.m_YAxisRecentering.m_enabled = false;
        }
        if (inputMoveVector.y >= -0.7 && inputMoveVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeCamera.m_YAxisRecentering.m_enabled = true;
        }
        else if (inputMoveVector.y < -0.95 && inputMoveVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeCamera.m_YAxisRecentering.m_enabled = false;
        }
        else if (inputMoveVector.y < -0.7 && inputMoveVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeCamera.m_YAxisRecentering.m_enabled = false;
        }
        else
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeCamera.m_YAxisRecentering.m_enabled = false;
        }

    }

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputMoveVector = context.ReadValue<Vector2>();
    }

    public void ActionLook(InputAction.CallbackContext context)
    {
        inputLookVector = context.ReadValue<Vector2>();
    }

    public void ActionFocus(InputAction.CallbackContext context)
    {
        inputMoveVector = context.ReadValue<Vector2>();
    }
}
