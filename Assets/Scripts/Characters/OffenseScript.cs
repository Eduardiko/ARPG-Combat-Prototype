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
        // Can The Player Attack?
        if(character.isGrounded)
            ableToAttack = true;
        else
            ableToAttack = false;
    }

    private void UpdateStatesAndAnimations()
    {
        // Attack
        if (inputManager.tryingToAttack && ableToAttack && !character.isPerformingAnAction)
        {
            inputManager.tryingToAttack = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger(character.animKeys.attackTriggerKey);
        }
        else
            inputManager.tryingToAttack = false;
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
