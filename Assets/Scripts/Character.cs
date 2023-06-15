using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // State Bools
    [HideInInspector] public bool isRunning = false;
    [HideInInspector] public bool isGrounded = false;
    [HideInInspector] public bool isLocking = false;

    // References that I need in more than one place, could be converted to the character parameters holder
    [HideInInspector] public GameObject target;

    private void Start()
    {
        target = new GameObject();
    }
}
