using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;
using System;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook freeCamera;

    [SerializeField] Transform playerLookAt;
    [SerializeField] private float lockDetectionRadius;
    [SerializeField] private LayerMask enemyLayerMask;

    private Vector2 inputMoveVector;
    private Vector2 inputLookVector;

    private bool tryingToLock;
    private bool isLocking = false;

    // Update is called once per frame
    void Update()
    {
        ManageRecentering();
        ManageLocking();
    }

    private void ManageRecentering()
    {
        //Recenter Camera If Walking Forwards
        if (inputLookVector != Vector2.zero)
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

    private void ManageLocking()
    {
        if (tryingToLock)
            isLocking = !isLocking;


        if(tryingToLock && isLocking)
        {
            tryingToLock = false;

            Collider[] hitColliders = Physics.OverlapSphere(freeCamera.m_Follow.position, lockDetectionRadius, enemyLayerMask);
            List<GameObject> nearbyEnemies = new List<GameObject>();

            foreach (Collider collider in hitColliders)
            {
                if(collider.gameObject.tag == "Enemy") nearbyEnemies.Add(collider.gameObject);
            }

            Vector3 closestDistance = nearbyEnemies[0].transform.position;
            GameObject nearestEnemy = new GameObject();

            foreach(GameObject enemy in nearbyEnemies)
            {
                Vector3 relativeDistance = freeCamera.m_Follow.transform.position - enemy.transform.position;

                if(relativeDistance.magnitude < closestDistance.magnitude) nearestEnemy = enemy;
            }

            freeCamera.m_LookAt = nearestEnemy.transform;
        } else if (tryingToLock)
        {
            tryingToLock = false;
            freeCamera.LookAt = playerLookAt;
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

    public void ActionLock(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToLock = true;
    }
}
