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
            ManageDamage(attackerCharacter.attackInfo);
        }
    }

    public void ManageDamage(AttackInfo attackInfo)
    {
     

        // Check if any of the two Weapon Angles is inside the threshold
        float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360f - attackInfo.topAngle));
        float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.bottomAngle, 360f - attackInfo.bottomAngle));

        if (angularTopDifference > 30)
            ReceiveDamage(attackInfo.damageAmmount);
        else if (angularTopDifference < 10)
            print("UN PARY LOCO");
        else
            print("Has been foken warded");
    }

    public void ReceiveDamage(float damageAmmount)
    {
        character.health -= damageAmmount;
        print(gameObject.name + "'s remaining health: " + character.health);
    }

    public void Parry()
    {
        // Trigger Parry To Other Man
    }
}
