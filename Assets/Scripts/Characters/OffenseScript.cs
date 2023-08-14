using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    [SerializeField] private GameObject weaponRDamager;
    [SerializeField] private GameObject weaponLDamager;

    private GameObject weaponDamager;
    private Vector3 damagerTopPos = new Vector3(0f, 1.3f, 0f);
    private Vector3 damagerBottomPos = new Vector3(0f, -0.7f, 0f);

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

    private bool isWeaponUpsideDown = false;

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
        if (!character.isMovementRestriced || character.isImmuneToDamage || character.isStaggered)
        {
            combo = 0;
            
            if(isWeaponUpsideDown)
            {
                character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
                character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
                isWeaponUpsideDown = false;
            }
        }

        // Reset Collider
        if(weaponDamager != null && character.isStaggered)
            DeactivateDamageCollider();

        // Weapon Top Attack
        if (inputManager.tryingToWeaponTopAttack && ableToAttack)
        {
            if(isWeaponUpsideDown)
            {
                character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
                character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
                isWeaponUpsideDown = false;
            }

            inputManager.tryingToWeaponTopAttack = false;
            Attack(weaponDial.topAngle, AttackType.SLASH_WEAPON_TOP);

            weaponDamager.transform.localPosition = damagerTopPos;
        }
        else
            inputManager.tryingToWeaponTopAttack = false;

        // Weapon Bottom Attack
        if (inputManager.tryingToWeaponBottomAttack && ableToAttack)
        {
            if(!isWeaponUpsideDown)
            {
                character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
                character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
                isWeaponUpsideDown = true;
            }

            inputManager.tryingToWeaponBottomAttack = false;
            Attack(weaponDial.bottomAngle, AttackType.SLASH_WEAPON_BOTTOM);

            weaponDamager.transform.localPosition = damagerBottomPos;
        }
        else
            inputManager.tryingToWeaponBottomAttack = false;

        // Weapon Thrust Attack
        if (inputManager.tryingToWeaponThrustAttack && ableToAttack)
        {
            if (isWeaponUpsideDown)
            {
                character.RWeapon.transform.Rotate(180f, 0f, 0f, Space.Self);
                character.LWeapon.transform.Rotate(0f, 0f, 180f, Space.Self);
                isWeaponUpsideDown = false;
            }

            inputManager.tryingToWeaponThrustAttack = false;
            ThrustAttack();

            weaponDamager.transform.localPosition = damagerTopPos;
        }
        else
            inputManager.tryingToWeaponThrustAttack = false;
    }

    private void Attack(float angle, AttackType type)
    {
        LookAtTarget();
        
        character.isUILocked = true;
        character.isMovementRestriced = true;

        if(!character.isLocking || (character.target.transform.position - transform.position).magnitude > 2f)
        {
            StopCoroutine(character.Step());
            StartCoroutine(character.Step());
        }
        
        attackSpamLimiterActive = true;
        

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

        if(angle <= 180f)
            UpdateWeapon(false, true);
        else
            UpdateWeapon(true, false);

        switch (attackSector)
        {
            case 3:
                UpdateWeapon(true, false);
                break;
            case 7:
                UpdateWeapon(false, true);
                break;
            default:
                break;
        }

        characterAnimator.SetInteger(character.animKeys.comboKey, combo);
        characterAnimator.SetFloat(character.animKeys.attackDirection, attackSector);
        characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);

        if(combo == 0)
            character.SetAttackInfo(10f, weaponDial.topAngle, weaponDial.bottomAngle, type);
        else
            character.SetAttackInfo(10f * combo, weaponDial.topAngle, weaponDial.bottomAngle, type);

        combo = combo + 1 > 2 ? 0 : combo + 1;
    }

    private void ThrustAttack()
    {
        LookAtTarget();

        character.isUILocked = true;
        character.isMovementRestriced = true;

        if (!character.isLocking || (character.target.transform.position - transform.position).magnitude > 2f)
        {
            StopCoroutine(character.Step());
            StartCoroutine(character.Step());
        }

        attackSpamLimiterActive = true;
        inputManager.bufferedAction = BufferActions.CLEAR;

        UpdateWeapon(false, true);
        characterAnimator.SetInteger(character.animKeys.comboKey, combo);
        characterAnimator.SetFloat(character.animKeys.attackDirection, 0f);
        characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);

        if (combo == 0)
            character.SetAttackInfo(5f, weaponDial.topAngle, weaponDial.bottomAngle, AttackType.THRUST);
        else
            character.SetAttackInfo(5f * combo, weaponDial.topAngle, weaponDial.bottomAngle, AttackType.THRUST);

        combo = combo + 1 > 2 ? 0 : combo + 1;
    }

    private void LookAtTarget()
    {
        // Rotate the character to the target one last time
        if (character.isLocking && character.target != null)
        {
            // Y axis to 0 so Vector is calculated at same height
            Vector3 targetPos = new Vector3(character.target.transform.position.x, 0f, character.target.transform.position.z);
            Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
            transform.rotation = Quaternion.LookRotation(targetPos - selfPos);
        }
    }

    private void ActivateDamageCollider()
    {
        character.isWeaponColliderActive = true;
        weaponDamager.SetActive(true);
    }

    private void DeactivateDamageCollider()
    {
        character.isWeaponColliderActive = false;
        weaponDamager.SetActive(false);
    }

    private void UpdateWeapon(bool lActive, bool rActive)
    {
        character.RWeapon.SetActive(rActive);
        character.LWeapon.SetActive(lActive);

        if (character.RWeapon.activeSelf)
            weaponDamager = weaponRDamager;
        else
            weaponDamager = weaponLDamager;
    }
}
