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

    public string backstepTriggerKey;
    public string dodgeRightTriggerKey;
    public string dodgeLeftTriggerKey;

    public string attackTriggerKey;

    // Bools
    public string isRunningKey;
    public string isLockingKey;

    // Floats
    public string directionXKey;
    public string directionZKey;
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

    // Access Character Animations
    [Header("Animations")]
    [SerializeField] public AnimationTriggerKeys animKeys;

    // State Bools - Changed By Code
    [HideInInspector] public bool isGrounded = true;
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isLocking = false;

    // State Bools - Changed By Animations
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isBackstepping = false;
    [HideInInspector] public bool isDodging = false;

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
        isBackstepping = false;
        isDodging = false;
    }

    private void IsNotAttacking()
    {
        isAttacking = false;
    }

    private void IsDodging()
    {
        isDodging = true;
        isBackstepping = false;
        isAttacking = false;
    }

    private void IsNotDodging()
    {
        isDodging = false;
    }
    
    #endregion

    #region HELPERS

    public void SetAttackInfo(float damageAmmount, float topAngle, float bottomAngle)
    {
        attackInfo.damageAmmount = damageAmmount;
        attackInfo.topAngle = topAngle;
        attackInfo.bottomAngle = bottomAngle;
    }

    #endregion
}
