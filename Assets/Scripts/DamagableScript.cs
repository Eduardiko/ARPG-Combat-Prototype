using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableScript : MonoBehaviour
{
    // References
    private Character character;
    private Animator characterAnimator;

    private void Start()
    {
        character = GetComponentInChildren<Character>();
        characterAnimator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon")
        {
            Character attackerCharacter = other.transform.root.gameObject.GetComponent<Character>();
            ReceiveDamage(attackerCharacter.damageAmmount);
        }
    }

    public void ReceiveDamage(float damageAmmount)
    {
        character.health -= damageAmmount;
        print(character.health);
    }
}
