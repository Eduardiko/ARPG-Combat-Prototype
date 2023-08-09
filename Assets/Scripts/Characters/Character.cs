using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Strings To Re-Use and Not memorize all animation parameters
[System.Serializable]
public class AnimationTriggerKeys
{
    // Triggers
    public string jumpTriggerKey;
    public string runTriggerKey;
    public string attackTriggerKey;
    public string dodgeTriggerKey;

    // Bools
    public string isRunningKey;
    public string isLockingKey;

    // Floats
    public string directionXKey;
    public string directionZKey;
    public string attackDirection;
    public string dodgeDirection;

}

public struct AttackInfo
{
    public float damageAmmount;
    public float topAngle;
    public float bottomAngle;
}

public class Character : MonoBehaviour
{
    // Character Parameters
    [Header("Parameters")]
    public float health = 100f;
    public Transform lookAtTransform;
    [SerializeField] private float stepLengthMultiplier = 1;
    [SerializeField] private float stepTime;

    // Access Character Animations
    [Header("Animations")]
    [SerializeField] public AnimationTriggerKeys animKeys;

    // State Bools - Changed By Code
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isLocking = false;

    // State Bools - Changed By Animations - To Have More Control
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isBackstepping = false;
    [HideInInspector] public bool isDodging = false;
    [HideInInspector] public bool isMovementRestriced = false;

    // State Bools - General Ones - Used when not caring about a specific state
    [HideInInspector] public bool isPerformingAnAction = false;
    [HideInInspector] public bool isImmuneToDamage = false;

    // Combat Information
    [HideInInspector] public AttackInfo attackInfo;
    [HideInInspector] public GameObject target;

    private void Start()
    {
        // True values will need to be here in Start() cause they can't be set beforehand idk why
        isGrounded = true;
        attackInfo.damageAmmount = 10;
    }

    private void Update()
    {
        UpdateGeneralBools();
    }

    #region STATES
    
    private void UpdateGeneralBools()
    {
        if (isAttacking || isBackstepping || isDodging)
            isPerformingAnAction = true;
        else
            isPerformingAnAction = false;


        if (isBackstepping || isDodging)
            isImmuneToDamage = true;
        else
            isImmuneToDamage = false;
    }

    private void IsAttacking()
    {
        isAttacking = true;
    }

    private void IsNotAttacking()
    {
        isAttacking = false;
        isDodging = false;
        isBackstepping = false;
    }

    private void IsDodging()
    {
        isDodging = true;
    }

    private void IsNotDodging()
    {
        isAttacking = false;
        isDodging = false;
        isBackstepping = false;
    }

    private void IsBackstepping()
    {
        isBackstepping = true;
    }

    private void IsNotBackstepping()
    {
        isAttacking = false;
        isDodging = false;
        isBackstepping = false;
    }

    private void IsMovementRestricted()
    {
        isMovementRestriced = true;
    }

    private void IsNotMovementRestricted()
    {
        isMovementRestriced = false;
    }

    #endregion

    #region HELPERS

    public void SetAttackInfo(float damageAmmount, float topAngle, float bottomAngle)
    {
        attackInfo.damageAmmount = damageAmmount;
        attackInfo.topAngle = topAngle;
        attackInfo.bottomAngle = bottomAngle;
    }

    public IEnumerator Step()
    {
        float elapsedTime = 0;

        // Rotate the character to the target one last time
        if(isLocking && target != null)
        {
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(target.transform.position.x, 0f, target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }

        while (elapsedTime < stepTime)
        {
            float stepDistance = Mathf.Lerp(stepLengthMultiplier, 0f, elapsedTime / stepTime);

            // Calculate the new position after stepping forward
            Vector3 newPosition = transform.position + transform.forward * stepDistance;

            // Move the GameObject to the new position
            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator BackStep(int direction=0)
    {
        float elapsedTime = 0;

        // Rotate the character to the target one last time
        if (isLocking && target != null)
        {
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(target.transform.position.x, 0f, target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }

        while (elapsedTime < stepTime)
        {
            float stepDistance = Mathf.Lerp(stepLengthMultiplier, 0f, elapsedTime / stepTime);

            Vector3 newPosition = new Vector3();

            switch (direction)
            {
                case 0:
                    // Calculate the new position after stepping forward
                    newPosition = transform.position - transform.forward * stepDistance * 2;
                    break;
                case 1:
                    // Calculate the new position after stepping forward
                    newPosition = transform.position + transform.right * stepDistance * 2;
                    break;
                case -1:
                    // Calculate the new position after stepping forward
                    newPosition = transform.position - transform.right * stepDistance * 2;
                    break;
                default:
                    break;
            }

            // Move the GameObject to the new position
            transform.position = newPosition;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}
