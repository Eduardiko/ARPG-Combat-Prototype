using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private CinemachineVirtualCamera lockCamera;

    [SerializeField] Transform playerLookAt;
    [SerializeField] private LayerMask enemyLayerMask;

    [SerializeField] Animator cinemachineAnimator;

    private Vector2 inputMoveVector;
    private Vector2 inputLookVector;

    private bool tryingToLock;
    private bool isLocking = false;

    private GameObject nearestEnemy;

    [Header("Settings")]
    [SerializeField] private bool zeroVertLook;
    [SerializeField] private float lockDetectionRadius;
    [SerializeField] private float lookAtSmoothing;
    [Tooltip("AngleDetection")] [SerializeField] private float maxLockAngle;
    //[SerializeField] private float crossHairScale;

    // Update is called once per frame
    void Update()
    {
        

        ManageRecentering();
        ManageLocking();
    }

    private void ManageRecentering()
    {
        //Recenter Camera If Walking Forwards
        if (isLocking)   
            return;
        

        if (inputLookVector != Vector2.zero)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
        }
        if (inputMoveVector.y >= -0.7 && inputMoveVector != Vector2.zero)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeLookCamera.m_YAxisRecentering.m_enabled = true;
        }
        else if (inputMoveVector.y < -0.95 && inputMoveVector != Vector2.zero)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
        }
        else if (inputMoveVector.y < -0.7 && inputMoveVector != Vector2.zero)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
        }
        else
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
        }
    }

    private void ManageLocking()
    {
        if (tryingToLock)
            isLocking = !isLocking;


        if(tryingToLock && isLocking)
        {
            tryingToLock = false;

            Collider[] hitColliders = Physics.OverlapSphere(freeLookCamera.m_Follow.position, lockDetectionRadius, enemyLayerMask);
            List<GameObject> nearbyEnemies = new List<GameObject>();

            // See video for polish on detection with angle

            foreach (Collider collider in hitColliders)
            {
                if(collider.gameObject.tag == "Enemy") nearbyEnemies.Add(collider.gameObject);
            }

            Vector3 closestDistance = freeLookCamera.m_Follow.transform.position - nearbyEnemies[0].transform.position;

            foreach(GameObject enemy in nearbyEnemies)
            {
                Vector3 relativeDistance = freeLookCamera.m_Follow.transform.position - enemy.transform.position;

                if(relativeDistance.magnitude <= closestDistance.magnitude) nearestEnemy = enemy;
            }

        } else if (tryingToLock)
        {
            tryingToLock = false;
            ResetLocking();
        }

        if(isLocking)
            SetLockedTarget();

    }

    private void SetLockedTarget()
    {

        lockCamera.LookAt = nearestEnemy.transform;

        Vector3 direction = nearestEnemy.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        lockCamera.transform.rotation = Quaternion.Slerp(lockCamera.transform.rotation, rotation, lookAtSmoothing * Time.deltaTime);

        freeLookCamera.gameObject.SetActive(false);
        lockCamera.gameObject.SetActive(true);

        //freeLookCamera.m_Priority = 1;
        //lockCamera.m_Priority = 2;
    }

    private void ResetLocking()
    {
        freeLookCamera.LookAt = playerLookAt;

        freeLookCamera.gameObject.SetActive(true);
        lockCamera.gameObject.SetActive(false);
        //freeLookCamera.m_Priority = 2;
        //lockCamera.m_Priority = 1;
    }

    public void ActionMove(InputAction.CallbackContext context)
    {
        inputMoveVector = context.ReadValue<Vector2>();
    }

    public void ActionLook(InputAction.CallbackContext context)
    {
        inputLookVector = context.ReadValue<Vector2>();
    }

    public void ActionLock(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToLock = true;
    }
}
