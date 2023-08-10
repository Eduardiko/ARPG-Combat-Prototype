using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    [SerializeField] private GameObject weaponDamager;


    // References
    private Character character;
    private InputManager inputManager;
    private Animator characterAnimator;
    private WeaponDial weaponDial;

    // Parameters
    private int combo = 0;
    

    // Bools
    private bool ableToAttack = false;

    // Comment - this limiter is added so that attacks don't perform
    //           animation cancelling at the start of another attack
    private bool attackSpamLimiterActive = false;

    private void Start()
    {
        characterAnimator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        character = GetComponent<Character>();
        weaponDial = GetComponent<WeaponDial>();
    }

    private void Update()
    {
        UpdatePossibleActions();
        UpdateStatesAndAnimations();
    }
    
    private void UpdatePossibleActions()
    {
        // If any action is successfully performed, the limiter is deactivated
        if (character.isPerformingAnAction)
            attackSpamLimiterActive = false;
        
        // Can The Player Attack?
        if(character.isGrounded && !character.isPerformingAnAction && !attackSpamLimiterActive)
            ableToAttack = true;
        else
            ableToAttack = false;
    }

    private void UpdateStatesAndAnimations()
    {
        // Reset Combo
        if (!character.isMovementRestriced || character.isImmuneToDamage)
            combo = 0;

        // Weapon Top Attack
        if (inputManager.tryingToWeaponTopAttack && ableToAttack)
        {
            inputManager.tryingToWeaponTopAttack = false;
            Attack(weaponDial.topAngle);
        }
        else
            inputManager.tryingToWeaponTopAttack = false;

        // Weapon Bottom Attack
        if (inputManager.tryingToWeaponBottomAttack && ableToAttack)
        {
            inputManager.tryingToWeaponBottomAttack = false;
            Attack(weaponDial.bottomAngle);
        }
        else
            inputManager.tryingToWeaponBottomAttack = false;
    }

    private void Attack(float angle)
    {
        StopCoroutine(character.Step());
        StartCoroutine(character.Step());
        attackSpamLimiterActive = true;
        combo = combo + 1 > 2 ? combo : combo + 1;

        inputManager.bufferedAction = BufferActions.CLEAR;

        float thresholdAngle = angle + 22.5f > 360f ? (angle + 22.5f - 360f) / 45f : (angle + 22.5f) / 45f;
        float attackSector = Mathf.Ceil(thresholdAngle);

        if(combo < 2)
        {
            switch (attackSector)
            {
                case 1:
                    attackSector = angle > 337.5f ? 8 : 2;
                    break;
                case 5:
                    attackSector = angle > 180 ? 6 : 4;
                    break;
                default:
                    break;
            }
        }

        characterAnimator.SetFloat(character.animKeys.attackDirection, attackSector);
        characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);

        character.SetAttackInfo(5f * combo, weaponDial.topAngle, weaponDial.bottomAngle);
    }

    private void ActivateDamageCollider()
    {
        weaponDamager.SetActive(true);
    }

    private void DeactivateDamageCollider()
    {
        weaponDamager.SetActive(false);
    }
}
