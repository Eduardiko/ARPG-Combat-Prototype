using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook freeCamera;
    private Vector2 inputVector;

    // Update is called once per frame
    void Update()
    {
        //Recenter Camera If Walking Forwards
        if (inputVector.y >= -0.7 && inputVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeCamera.m_YAxisRecentering.m_enabled = true;
        }
        else if (inputVector.y < -0.95 && inputVector != Vector2.zero)
        {
            freeCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeCamera.m_YAxisRecentering.m_enabled = false;
        }
        else if (inputVector.y < -0.7 && inputVector != Vector2.zero)
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

    public void ActionLook(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }
}
