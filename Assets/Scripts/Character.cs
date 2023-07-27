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

public class Character : MonoBehaviour
{
    // Acess Character Animations
    [Header("Animations")]
    [SerializeField] public AnimationTriggerKeys animKeys;

    // State Bools - Changed By Code
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isLocking = false;

    // State Bools - Changed By Animations
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isBackstepping = false;
    [HideInInspector] public bool isDodging = false;

    // State Bools - General Ones - Used when not caring about a specific state
    [HideInInspector] public bool isPerformingAnAction = false;
    [HideInInspector] public bool isImmuneToDamage = false;

    // Character Parameters
    [HideInInspector] public float health = 100f;

    // Attack Information
    [HideInInspector] public float damageAmmount = 10f;


    // References that I need in more than one place, could be converted to the character parameters holder
    [HideInInspector] public GameObject target;

    private void Start()
    {
        target = new GameObject();
    }

    private void Update()
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

}
