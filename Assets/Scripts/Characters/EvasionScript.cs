using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvasionScript : MonoBehaviour
{

    // References
    private Character character;
    private InputManager inputManager;
    private Animator characterAnimator;

    // Bools
    private bool ableToBackstep = false;
    private bool ableToDodge = false;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        inputManager = GetComponent<InputManager>();
        characterAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePossibleActions();
        UpdateStatesAndAnimations();
    }

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
        {
            inputManager.tryingToBackstep = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetFloat(character.animKeys.dodgeDirection, 0f);
            characterAnimator.SetTrigger(character.animKeys.dodgeTriggerKey);
        }
        else
            inputManager.tryingToBackstep = false;

        // Dodge Right
        if (inputManager.tryingToDodgeRight && ableToDodge)
        {
            inputManager.tryingToDodgeRight = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetFloat(character.animKeys.dodgeDirection, 1f);
            characterAnimator.SetTrigger(character.animKeys.dodgeTriggerKey);
        }
        else
            inputManager.tryingToDodgeRight = false;

        // Dodge Left
        if (inputManager.tryingToDodgeLeft && ableToDodge)
        {
            inputManager.tryingToDodgeLeft = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetFloat(character.animKeys.dodgeDirection, -1f);
            characterAnimator.SetTrigger(character.animKeys.dodgeTriggerKey);
        }
        else
            inputManager.tryingToDodgeLeft = false;
    }

}
