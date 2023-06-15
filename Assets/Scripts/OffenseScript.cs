using UnityEngine;
using UnityEngine.InputSystem;

public class OffenseScript : MonoBehaviour
{
    private Animator characterAnimator;

    private bool tryingToAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        characterAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(tryingToAttack)
        {
            tryingToAttack = false;
            characterAnimator.SetTrigger("AttackTrigger");
        }
    }

    public void ActionAttack(InputAction.CallbackContext context)
    {
        if (context.performed) tryingToAttack = true;
    }
}
