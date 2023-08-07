    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    // References
    private InputManager inputManager;
    private Character character;
    public GameObject player;

    // Variables
    [SerializeField] private float actionTriggerTime = 5f;
    private float actionTriggerTimer = 0f;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        character = GetComponent<Character>();
        character.isLocking = true;
        character.target = player;
    }

    // Update is called once per frame
    void Update()
    {
        if(actionTriggerTimer < 0f)
        {
            actionTriggerTimer = actionTriggerTime;
            inputManager.tryingToWeaponTopAttack = true;
            inputManager.inputMoveVector = new Vector2(0.01f, 0.01f);
        } else
        {
            actionTriggerTimer -= Time.deltaTime;
        }
    }
}
