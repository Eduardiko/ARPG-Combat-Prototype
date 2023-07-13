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

    // State Bools
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isLocking = false;
    [HideInInspector] public bool isBackstepping = false;
    [HideInInspector] public bool isDodging = false;

    // References that I need in more than one place, could be converted to the character parameters holder
    [HideInInspector] public GameObject target;

    private void Start()
    {
        target = new GameObject();
    }
}
