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
    }

    void Update()
    {
        if(inputManager.tryingToAttack)
        {
            inputManager.tryingToAttack = false;
            characterAnimator.SetTrigger("AttackTrigger");
        }
    }
}
