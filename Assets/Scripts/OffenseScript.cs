using UnityEngine;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    // References
    private Animator characterAnimator;
    private InputManager inputManager;
    private Character character;

    void Start()
    {
        characterAnimator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        character = GetComponent<Character>();
    }

    void Update()
    {
        if(inputManager.tryingToAttack && character.isGrounded)
        {
            inputManager.tryingToAttack = false;
            inputManager.bufferedAction = BufferActions.CLEAR;
            characterAnimator.SetTrigger("AttackTrigger");
        }
        else
            inputManager.tryingToAttack = false;
    }
}
