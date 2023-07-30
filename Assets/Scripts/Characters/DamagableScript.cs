using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableScript : MonoBehaviour
{
    // References
    private Character character;
    private Animator characterAnimator;
    private WeaponDial weaponDial;

    private void Start()
    {
        character = GetComponentInChildren<Character>();
        characterAnimator = GetComponentInChildren<Animator>();
        weaponDial = GetComponent<WeaponDial>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon" && other.transform.root.gameObject != gameObject && !character.isImmuneToDamage)
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
