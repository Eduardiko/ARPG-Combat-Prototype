using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionScript : MonoBehaviour
{

    // References
    private Character character;
    private InputManager inputManager;

    // Bools
    private bool ableToBackstep = false;
    private bool ableToDodge = false;


    void Start()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        if (!character.isDead)
            UpdatePossibleActions();

        UpdateStatesAndAnimations();
    }

    #region MAIN
    private void UpdatePossibleActions()
    {
        // Can The Player Backstep?
        if (character.isGrounded && !character.isPerformingAnAction)
            ableToBackstep = true;
        else
            ableToBackstep = false;

        // Can The Player Dodge?
        if (character.isGrounded && !character.isPerformingAnAction)
            ableToDodge = true;
        else
            ableToDodge = false;
    }

    private void UpdateStatesAndAnimations()
    {
        // Backstep
        if (inputManager.tryingToBackstep && ableToBackstep)
            BackStep();
        else
            inputManager.tryingToBackstep = false;

        // Dodge Right
        if (inputManager.tryingToDodgeRight && ableToDodge)
            DodgeRight();
        else
            inputManager.tryingToDodgeRight = false;

        // Dodge Left
        if (inputManager.tryingToDodgeLeft && ableToDodge)
            DodgeLeft();
        else
            inputManager.tryingToDodgeLeft = false;
    }

    #endregion

    #region ACTIONS

    private void BackStep()
    {
        // Update States
        character.isBackstepping = true;
        character.isImmuneToDamage = true;
        character.isMovementRestriced = true;
        
        // Clear Input
        inputManager.tryingToBackstep = false;
        inputManager.bufferedAction = BufferActions.CLEAR;

        // Perform Step
        StopCoroutine(character.BackStep());
        StartCoroutine(character.BackStep());

        // Set Animation
        character.animator.SetFloat(character.animKeys.dodgeDirection, 0f);
        character.animator.SetTrigger(character.animKeys.dodgeTriggerKey);
    }

    private void DodgeRight()
    {
        // Update States
        character.isDodging = true;
        character.isImmuneToDamage = true;
        character.isMovementRestriced = true;

        // Clear Input
        inputManager.tryingToDodgeRight = false;
        inputManager.bufferedAction = BufferActions.CLEAR;

        // Perform Step
        StopCoroutine(character.BackStep());
        StartCoroutine(character.BackStep(1));

        // Update hand weapon
        character.UpdateWeapon(false, true);

        // Set Animation
        character.animator.SetFloat(character.animKeys.dodgeDirection, 1f);
        character.animator.SetTrigger(character.animKeys.dodgeTriggerKey);
    }

    private void DodgeLeft()
    {
        // Update States
        character.isDodging = true;
        character.isImmuneToDamage = true;
        character.isMovementRestriced = true;

        // Clear Input
        inputManager.tryingToDodgeLeft = false;
        inputManager.bufferedAction = BufferActions.CLEAR;

        // Perform Step
        StopCoroutine(character.BackStep());
        StartCoroutine(character.BackStep(-1));

        // Update hand weapon
        character.UpdateWeapon(true, false);

        // Set Animation
        character.animator.SetFloat(character.animKeys.dodgeDirection, -1f);
        character.animator.SetTrigger(character.animKeys.dodgeTriggerKey);
    }

    #endregion

}
