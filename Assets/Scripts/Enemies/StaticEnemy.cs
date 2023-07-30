    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    // References
    private InputManager inputManager;

    // Variables
    [SerializeField] private float actionTriggerTime = 5f;
    private float actionTriggerTimer = 0f;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(actionTriggerTimer < 0f)
        {
            actionTriggerTimer = actionTriggerTime;
            inputManager.tryingToAttack = true;
        } else
        {
            actionTriggerTimer -= Time.deltaTime;
        }
    }
}
