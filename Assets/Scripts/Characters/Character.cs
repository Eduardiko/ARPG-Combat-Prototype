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
    public string hitTriggerKey;

    // Bools
    public string isRunningKey;
    public string isLockingKey;

    // Floats
    public string directionXKey;
    public string directionZKey;
    public string attackDirection;
    public string dodgeDirection;
    public string hitID;

    // Ints
    public string comboKey;
}

public enum AttackType
{
    SLASH_WEAPON_TOP,
    SLASH_WEAPON_BOTTOM,
    THRUST
}

public struct AttackInfo
{
    public float damageAmmount;
    public float topAngle;
    public float bottomAngle;
    public AttackType type;
}

public class Character : MonoBehaviour
{
    // Character Parameters
    [Header("Parameters")]
    public float health = 100f;
    public Transform lookAtTransform;
    [SerializeField] private float attackStepLengthMultiplier = 1;
    [SerializeField] private float attackStepTime;
    [SerializeField] private float stepLengthMultiplier = 1;
    [SerializeField] private float stepTime;

    // Access Character Animations
    [Header("Animations")]
    [SerializeField] public AnimationTriggerKeys animKeys;
    [HideInInspector] public Animator characterAnimator;

    // Fake It Until U Make It References
    public GameObject RWeapon;
    public GameObject LWeapon;

    // State Bools - Changed By Code
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isLocking = false;
    [HideInInspector] public bool isDead = false;

    // State Bools - Changed By Animations - To Have More Control
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isBackstepping = false;
    [HideInInspector] public bool isDodging = false;
    [HideInInspector] public bool isMovementRestriced = false;
    [HideInInspector] public bool isStaggered = false;

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
        attackInfo.topAngle = 180f;
        characterAnimator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateGeneralBools();
    }

    #region STATES
    
    private void UpdateGeneralBools()
    {
        if (isAttacking || isBackstepping || isDodging || isStaggered || isDead)
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
    }

    private void IsDodging()
    {
        isDodging = true;
    }

    private void IsNotDodging()
    {
        isDodging = false;
    }

    private void IsBackstepping()
    {
        isBackstepping = true;
    }

    private void IsNotBackstepping()
    {
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

    private void IsStaggered()
    {
        isStaggered = true;
        isAttacking = false;
        isDodging = false;
        isBackstepping = false;
    }

    private void IsNotStaggered()
    {
        isStaggered = false;
    }

    private void IsDead()
    {
        isDead = true;
    }

    #endregion

    #region HELPERS

    public void SetAttackInfo(float damageAmmount, float topAngle, float bottomAngle, AttackType type)
    {
        attackInfo.damageAmmount = damageAmmount;
        attackInfo.topAngle = topAngle;
        attackInfo.bottomAngle = bottomAngle;
        attackInfo.type = type;
    }

    public IEnumerator Step()
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

        while (elapsedTime < attackStepTime)
        {
            float stepDistance = Mathf.Lerp(attackStepLengthMultiplier, 0f, elapsedTime / stepTime);

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
