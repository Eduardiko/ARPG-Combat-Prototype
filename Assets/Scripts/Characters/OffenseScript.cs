using UnityEngine;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    [SerializeField] private GameObject weaponDamager;
    
    // References
    private Character character;
    private InputManager inputManager;
    private Animator characterAnimator;
    private WeaponDial weaponDial;
    
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
        
        // Weapon Top Attack
        if (inputManager.tryingToWeaponTopAttack && ableToAttack)
        {
            attackSpamLimiterActive = true;

            // Division of the Weapon Dial in 8 parts with a 22.5 degree offset to set different animations 
            float thresholdAngle = weaponDial.topAngle + 22.5f > 360f ? (weaponDial.topAngle + 22.5f - 360f) / 45f : (weaponDial.topAngle + 22.5f) / 45f;
            float attackSector = Mathf.Ceil(thresholdAngle);

            inputManager.tryingToWeaponTopAttack = false;
            inputManager.bufferedAction = BufferActions.CLEAR;

            characterAnimator.SetFloat(character.animKeys.attackDirection, attackSector);
            characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);

            character.SetAttackInfo(5f, weaponDial.topAngle, weaponDial.bottomAngle);
        }
        else
            inputManager.tryingToWeaponTopAttack = false;

        // Weapon Bottom Attack
        if (inputManager.tryingToWeaponBottomAttack && ableToAttack)
        {
            attackSpamLimiterActive = true;

            float thresholdAngle = weaponDial.bottomAngle + 22.5f > 360f ? (weaponDial.bottomAngle + 22.5f - 360f) / 45f : (weaponDial.bottomAngle + 22.5f) / 45f;
            float attackSector = Mathf.Ceil(thresholdAngle);

            inputManager.tryingToWeaponBottomAttack = false;
            inputManager.bufferedAction = BufferActions.CLEAR;

            characterAnimator.SetFloat(character.animKeys.attackDirection, attackSector);
            characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);

            character.SetAttackInfo(5f, weaponDial.topAngle, weaponDial.bottomAngle);
        }
        else
            inputManager.tryingToWeaponBottomAttack = false;
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
