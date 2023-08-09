    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : MonoBehaviour
{
    // References
    private InputManager inputManager;
    private WeaponDial weaponDial;
    private Character character;
    public GameObject player;

    // Variables
    [SerializeField] private float actionTriggerTime = 5f;
    private float actionTriggerTimer = 0f;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        weaponDial = GetComponent<WeaponDial>();
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

            // Force a random angle for each attack
            float randomAngle = Random.Range(Random.Range(0f, 360f), Random.Range(0f, 360f));
            float oppositeAngle = randomAngle + 180 > 360 ? randomAngle - 180 : randomAngle + 180;

            weaponDial.isUIWeaponAttached = true;
            weaponDial.manualAngle = randomAngle;
            weaponDial.topAngle = randomAngle;
            weaponDial.bottomAngle = oppositeAngle;

            inputManager.inputMoveVector = new Vector2(0.01f, 0.01f);
        } else
        {
            actionTriggerTimer -= Time.deltaTime;
        }
    }
}
