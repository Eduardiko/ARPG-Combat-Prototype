using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject weaponRDamager;
    [SerializeField] private GameObject weaponLDamager;

    // References
    private Character character;
    private InputManager inputManager;
    private WeaponDial weaponDial;

    // Variables
    private int combo = 0;
    private float attackSector = 0f;

    private GameObject weaponDamager;

    private Vector3 damagerTopPos = new Vector3(0f, 1.3f, 0f);
    private Vector3 damagerBottomPos = new Vector3(0f, -0.7f, 0f);
    
    // Bools
    private bool ableToAttack = false;
    private bool isWeaponUpsideDown = false;

    // Comment - this limiter is added so that attacks don't perform
    //           animation cancelling at the start of another attack
    private bool attackSpamLimiterActive = false;


    private void Start()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
        weaponDial = GetComponent<WeaponDial>();

        weaponDamager = weaponRDamager;
    }

    private void Update()
    {
        if(!character.isDead)
        { 
            UpdatePossibleActions();
            UpdateStatesAndAnimations();
        }
    }

    #region MAIN

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
        // Check for reset
        if (!character.isMovementRestriced || character.isStaggered)
            PeformResets();

        // Weapon Top Attack
        if (inputManager.tryingToWeaponTopAttack && ableToAttack)
            Attack(AttackType.SLASH_WEAPON_TOP, weaponDial.topAngle);
        else
            inputManager.tryingToWeaponTopAttack = false;

        // Weapon Bottom Attack
        if (inputManager.tryingToWeaponBottomAttack && ableToAttack)
            Attack(AttackType.SLASH_WEAPON_BOTTOM, weaponDial.bottomAngle);
        else
            inputManager.tryingToWeaponBottomAttack = false;

        // Weapon Thrust Attack
        if (inputManager.tryingToWeaponThrustAttack && ableToAttack)
            Attack(AttackType.THRUST);
        else
            inputManager.tryingToWeaponThrustAttack = false;
    }

    #endregion

    #region ATTACK

    private void Attack(AttackType type, float angle=0f)
    {
        // Update States
        character.isMovementRestriced = true;

        inputManager.tryingToWeaponTopAttack = false;
        inputManager.bufferedAction = BufferActions.CLEAR;

        attackSpamLimiterActive = true;

        // Rotate the character to the target
        if (character.isLocking && character.target != null)
            LookAtTarget();

        // Perform a step
        StopCoroutine(character.Step());
        StartCoroutine(character.Step());

        // Calculate sector to define animation - 8 sectors & 22.5 degree offset
        if (type != AttackType.THRUST)
        {
            float thresholdAngle = angle + 22.5f > 360f ? (angle + 22.5f - 360f) / 45f : (angle + 22.5f) / 45f;
            attackSector = Mathf.Ceil(thresholdAngle);
        }
        else
            attackSector = 0f;

        // Change weapon's hand, rotate and adjust collider
        UpdateWeapon(type, angle);

        // Set Animation
        character.animator.SetInteger(character.animKeys.comboKey, combo);
        character.animator.SetFloat(character.animKeys.attackDirection, attackSector);
        character.animator.SetTrigger(character.animKeys.attackTriggerKey);

        // Set combo
        if(combo == 0)
            character.SetAttackInfo(10f, weaponDial.topAngle, weaponDial.bottomAngle, type);
        else
            character.SetAttackInfo(10f * combo, weaponDial.topAngle, weaponDial.bottomAngle, type);

        combo = combo + 1 > 2 ? 0 : combo + 1;
    }

    #endregion

    #region WEAPON

    private void UpdateWeapon(AttackType type, float angle = 0f)
    {
        if (type != AttackType.THRUST)
        {
            // Hardcoded - As I didn't find light top/bottom attack animations, it will perform light high/low attacks
            if (combo < 2)
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

            // Switch between Left/Right hand depending on Left/Right part of the dial
            if (angle <= 180f)
                UpdateWeaponHand(false, true);
            else
                UpdateWeaponHand(true, false);

            // Hardcoded - For Light/Strong Mid Attack animations, the hand needs to be inversed for the combos to work
            switch (attackSector)
            {
                case 3:
                    UpdateWeaponHand(true, false);
                    break;
                case 7:
                    UpdateWeaponHand(false, true);
                    break;
                default:
                    break;
            }
        }
        else
            UpdateWeaponHand(false, true);

        // Rotate Weapon and Move Collider
        if (type != AttackType.SLASH_WEAPON_BOTTOM)
        {
            if (isWeaponUpsideDown)
                SwitchWeaponRotation();

            weaponDamager.transform.localPosition = damagerTopPos;
        }
        else
        {
            if (!isWeaponUpsideDown)
                SwitchWeaponRotation();

            weaponDamager.transform.localPosition = damagerBottomPos;
        }
    }

    private void UpdateWeaponHand(bool lActive, bool rActive)
    {
        character.UpdateWeapon(lActive, rActive);

        if (character.RWeapon.activeSelf)
            weaponDamager = weaponRDamager;
        else
            weaponDamager = weaponLDamager;
    }

    private void SwitchWeaponRotation()
    {
        if (isWeaponUpsideDown)
        {
            character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
            character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
            isWeaponUpsideDown = false;
        }
        else
        {
            character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
            character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
            isWeaponUpsideDown = true;
        }
    }

    #endregion

    #region HELPERS

    private void LookAtTarget()
    {
        // Y axis to 0 so Vector is calculated at same height
        Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
        Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
        transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
    }

    private void PeformResets()
    {
        // Reset Combo
        combo = 0;

        // Return Weapon to default
        if (isWeaponUpsideDown)
        {
            character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
            character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
            isWeaponUpsideDown = false;
        }

        // Reset Collider
        if (weaponDamager != null)
            DeactivateDamageCollider();
    }

    private void ActivateDamageCollider()
    {
        character.isWeaponColliderActive = true;
        weaponDamager.SetActive(true);
    }

    private void DeactivateDamageCollider()
    {
        character.ClearAttackInfo();
        character.isWeaponColliderActive = false;
        weaponDamager.SetActive(false);
    }

    #endregion
}
