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
        if (inputManager.tryingToBackstep && character.isGrounded)
            ableToBackstep = true;
        else
            ableToBackstep = false;

        // Can The Player Dodge?
        if ((inputManager.tryingToDodgeRight || inputManager.tryingToDodgeLeft) && character.isGrounded)
            ableToDodge = true;
        else
            ableToDodge = false;
    }

    private void UpdateStatesAndAnimations()
    {
        if (inputManager.tryingToBackstep && ableToBackstep)
        {
            inputManager.tryingToBackstep = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger("BackstepTrigger");
        }

        if (inputManager.tryingToDodgeRight && ableToDodge)
        {
            inputManager.tryingToDodgeRight = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger("DodgeRightTrigger");
        }

        if (inputManager.tryingToDodgeLeft && ableToDodge)
        {
            inputManager.tryingToDodgeLeft = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger("DodgeLeftTrigger");
        }
    }

}
