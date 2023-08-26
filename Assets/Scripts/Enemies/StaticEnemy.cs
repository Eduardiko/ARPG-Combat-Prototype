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
    [SerializeField] private bool nonAgressive;
    [SerializeField] private bool isInvincible;
    [SerializeField] private bool forceThrust;
    [SerializeField] private bool forceBottom;
    [SerializeField] private float actionTriggerTime = 5f;
    private float actionTriggerTimer = 0f;

    public float forcedAngle = 0f;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        weaponDial = GetComponent<WeaponDial>();
        character = GetComponent<Character>();
        character.target = player;
    }

    // Update is called once per frame
    void Update()
    {
        character.isLocking = true;

        if (!nonAgressive)
        {
            if (actionTriggerTimer < 0f)
            {
                actionTriggerTimer = actionTriggerTime;
                
                if(!forceThrust)
                {
                    if(forceBottom)
                        inputManager.tryingToWeaponBottomAttack = true;
                    else
                        inputManager.tryingToWeaponTopAttack = true;


                    // Force a random angle for each attack
                    float randomAngle = Random.Range(Random.Range(0f, 360f), Random.Range(0f, 360f));

                    if (forcedAngle != 0f)
                        randomAngle = forcedAngle;

                    float oppositeAngle = randomAngle + 180 > 360 ? randomAngle - 180 : randomAngle + 180;

                    weaponDial.isUIWeaponAttached = true;
                    weaponDial.manualAngle = randomAngle;
                    weaponDial.topAngle = randomAngle;
                    weaponDial.bottomAngle = oppositeAngle;
                }
                else
                    inputManager.tryingToWeaponThrustAttack = true;

            }
            else
            {
                actionTriggerTimer -= Time.deltaTime;
            }
        }
        
        if(isInvincible)
        {
            if (character.health < 25f)
                character.health = character.maxHealth;
        }

    }
}
