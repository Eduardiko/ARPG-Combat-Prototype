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

    private Vector2 inputMoveVector;
    private Vector2 inputLookVector;

    private bool tryingToLock;
    private bool isLocking = false;

    private GameObject nearestEnemy;
    private List<GameObject> lockableEnemies = new List<GameObject>();
    [SerializeField] private LayerMask enemyLayerMask;

    [Header("Settings")]
    [SerializeField] private float lockDetectionRadius;
    [SerializeField] private float lookAtSmoothing;
    [SerializeField] private float maxLockAngle;

    private float initialFreeLookX;
    private float initialFreeLookY;

    private void Start()
    {
        initialFreeLookX = freeLookCamera.m_XAxis.Value;
        print(initialFreeLookX);
        initialFreeLookY = freeLookCamera.m_YAxis.Value;
        print(initialFreeLookY);

    }

    void Update()
    {
        ManageRecentering();
        ManageLocking();
    }

    #region MANAGERS
    private void ManageRecentering()
    {
        //Locking && Looking - have priority over walking
        if (isLocking || inputLookVector != Vector2.zero)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
            return;
        }

        // Walking
        if (inputMoveVector == Vector2.zero || inputMoveVector.y < -0.95)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = false;
            freeLookCamera.m_YAxisRecentering.m_enabled = false;
        }
        else if (inputMoveVector.y >= -0.7)
        {
            freeLookCamera.m_RecenterToTargetHeading.m_enabled = true;
            freeLookCamera.m_YAxisRecentering.m_enabled = true;
        }
        else if (inputMoveVector.y < -0.7)
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
        if (tryingToLock && !isLocking)
        {
            tryingToLock = false;

            // If there are not available enemies, don't change the camera
            if (FindLockableTargets())
            {
                isLocking = true;
                SetLockTarget();
                SetLockCamera();
            }
        }
        else if (tryingToLock && isLocking)
        {
            // If locking, return to freeLook
            tryingToLock = false;
            isLocking = false;
            SetFreeLookCamera();
        }

        // Updates camera so player is always in-sight
        if (isLocking)
            UpdateLockedCamera();

    }
    #endregion

    #region HELPERS
    private bool FindLockableTargets()
    {
        lockableEnemies.Clear();

        // Find all nearby GameObjects && LookAt Vector of the camera
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, lockDetectionRadius, enemyLayerMask);
        Vector3 lookDir = freeLookCamera.m_LookAt.transform.position - freeLookCamera.transform.position;

        foreach (Collider collider in hitColliders)
        {
            // Calculate angle between the relative direction w/enemy and the "center of screen" vectors to determine if they are inside the range
            Vector3 dir = collider.gameObject.transform.position - freeLookCamera.transform.position;
            float angle = Vector3.Angle(lookDir, dir);

            if (collider.gameObject.tag == "Enemy" && angle < maxLockAngle)
            {
                lockableEnemies.Add(collider.gameObject);
            }
        }

        if (lockableEnemies.Count == 0) return false;

        return true;
    }
    private void UpdateLockedCamera()
    {
        lockCamera.LookAt = nearestEnemy.transform;

        // Rotates the camera so that the forward Vector is always the Vector between enemy & player
        Vector3 direction = nearestEnemy.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        lockCamera.transform.rotation = Quaternion.Slerp(lockCamera.transform.rotation, rotation, lookAtSmoothing * Time.deltaTime);
    }

    #endregion

    #region SETTERS
    private void SetFreeLookCamera()
    {
        freeLookCamera.gameObject.SetActive(true);
        lockCamera.gameObject.SetActive(false);
    }

    private void SetLockCamera()
    {
        lockCamera.gameObject.SetActive(true);
        freeLookCamera.gameObject.SetActive(false);
    }

    private void SetLockTarget()
    {
        // Calculates the nearest enemy from the list of available locking enemies
        Vector3 closestDistance = freeLookCamera.m_Follow.transform.position - lockableEnemies[0].transform.position;

        foreach (GameObject enemy in lockableEnemies)
        {
            Vector3 relativeDistance = freeLookCamera.m_Follow.transform.position - enemy.transform.position;
            if (relativeDistance.magnitude <= closestDistance.magnitude) nearestEnemy = enemy;
        }
    }
    #endregion

    #region ACTIONS
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
    #endregion
}
