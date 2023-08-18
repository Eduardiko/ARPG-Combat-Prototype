    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartEnemy : MonoBehaviour
{
    // References
    public GameObject player;
    private Character character;
    private InputManager inputManager;
    private WeaponDial weaponDial;
    private CharacterController characterController;

    // Variables
    [SerializeField] private float actionAttackMaxTriggerTime = 5f;
    [SerializeField] private float actionAttackMinTriggerTime = 5f;
    private float actionAttackTriggerTime = 1f;

    [Range(0.0f, 100.0f)]
    [SerializeField] private float thrustAttackProbability = 5f;

    public float forcedAngle = 0f;

    public float moveSpeed = 0.5f;

    private Vector3 initialPosition;

    private Vector3 toTargetVector;
    private Vector3 toInitVector;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        weaponDial = GetComponent<WeaponDial>();
        character = GetComponent<Character>();
        characterController = GetComponent<CharacterController>();

        character.target = player;

        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(!character.isPerformingAnAction)
            MoveToTarget();

        if (character.isLocking)
            Attack();
    }

    private void MoveToTarget()
    {
        toTargetVector = character.target.transform.position - transform.position;
        toInitVector = initialPosition - transform.position;

        if (toTargetVector.magnitude > 5f && toTargetVector.magnitude < 12f)
        {
            character.isLocking = false;
            toTargetVector = toTargetVector.normalized * moveSpeed * Time.deltaTime;
            character.animator.SetFloat(character.animKeys.directionZKey, 1f);
            characterController.Move(toTargetVector);

            // Rotate the player to face the target
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);

            actionAttackTriggerTime = 1f;
        }
        else if (toTargetVector.magnitude >= 12f && toInitVector.magnitude > 0.2f)
        {
            character.isLocking = false;
            toInitVector = toInitVector.normalized * moveSpeed * Time.deltaTime;
            character.animator.SetFloat(character.animKeys.directionZKey, 1f);
            characterController.Move(toInitVector);

            // Rotate the player to face the target
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(initialPosition.x, 0f, initialPosition.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }
        else if(toTargetVector.magnitude <= 5f && toTargetVector.magnitude > 2f)
        {
            character.isLocking = true;
            toTargetVector = toTargetVector.normalized * moveSpeed/10f * Time.deltaTime;
            character.animator.SetFloat(character.animKeys.directionZKey, 0.4f);
            characterController.Move(toTargetVector);

            // Rotate the player to face the target
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }
        else
        {
            character.animator.SetFloat(character.animKeys.directionZKey, 0f);

            if (toTargetVector.magnitude <= 3f)
            {
                character.isLocking = true;
            }
        }
    }

    private void Attack()
    {
        if (!character.isMovementRestriced)
        {
            // Rotate the player to face the target
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }

        if (actionAttackTriggerTime < 0f)
        {
            actionAttackTriggerTime = Random.Range(actionAttackMinTriggerTime, actionAttackMaxTriggerTime);

            float randomType = Random.Range(0f, 100f);
            if (randomType <= thrustAttackProbability)
                inputManager.tryingToWeaponThrustAttack = true;
            else
            {
                // Force a random angle for each attack
                float randomAngle = Random.Range(Random.Range(0f, 360f), Random.Range(0f, 360f));

                if (forcedAngle != 0f)
                    randomAngle = forcedAngle;

                float oppositeAngle = randomAngle + 180 > 360 ? randomAngle - 180 : randomAngle + 180;

                weaponDial.manualAngle = randomAngle;
                weaponDial.topAngle = randomAngle;
                weaponDial.bottomAngle = oppositeAngle;

                inputManager.tryingToWeaponTopAttack = true;
            }

            weaponDial.isUIWeaponAttached = true;
        }
        else
        {
            actionAttackTriggerTime -= Time.deltaTime;
        }
    }
}
