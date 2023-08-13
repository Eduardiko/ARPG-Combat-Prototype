using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableScript : MonoBehaviour
{
    // References
    private Character character;
    private Animator characterAnimator;
    private WeaponDial weaponDial;

    private float damageResetTime = 0.3f;
    private float damageResetTimer = 0.3f;

    private void Start()
    {
        character = GetComponentInChildren<Character>();
        characterAnimator = GetComponentInChildren<Animator>();
        weaponDial = GetComponent<WeaponDial>();
    }

    private void Update()
    {
        if(damageResetTimer < damageResetTime)
            damageResetTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon" && other.transform.root.gameObject != gameObject && !character.isImmuneToDamage && damageResetTimer >= damageResetTime)
        {
            Character attackerCharacter = other.transform.root.gameObject.GetComponent<Character>();
            ManageDamage(attackerCharacter);
            damageResetTimer = 0f;
        }
    }

    public void ManageDamage(Character attackerCharacter)
    {
        // Check if any of the two Weapon Angles is inside the threshold
        float angularTopDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360f - attackerCharacter.attackInfo.topAngle));
        float angularBottomDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.bottomAngle, 360f - attackerCharacter.attackInfo.bottomAngle));

        if (angularTopDifference > 30)
            ReceiveDamage(attackerCharacter);
        else if (angularTopDifference < 10)
            Parry();
        else
            Guard();
    }

    public void ReceiveDamage(Character attackerCharacter)
    {
        character.health -= attackerCharacter.attackInfo.damageAmmount;
        print(gameObject.name + "'s remaining health: " + character.health);

        if (character.health < 0)
        {
            Die(attackerCharacter);
            return;
        } 

        characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);
        float randomAnimID = Random.Range(1f, 5f);
        characterAnimator.SetFloat(character.animKeys.hitID, randomAnimID);
    }

    public void Guard()
    {
        print("guard");
        characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);

        float randomAnimID = Random.Range(6f, 8f);
        characterAnimator.SetFloat(character.animKeys.hitID, randomAnimID);
    }

    public void Parry()
    {
        print("parry");
        characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);

        //float randomAnimID = Random.Range(6f, 8f);
        characterAnimator.SetFloat(character.animKeys.hitID, 9f);
    }

    public void Die(Character attackerCharacter)
    {
        attackerCharacter.isLocking = false;
        characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);
        float randomAnimID = Mathf.Round(Random.Range(11f, 12f));
        characterAnimator.SetFloat(character.animKeys.hitID, randomAnimID);
        gameObject.tag = "Corpse";
        gameObject.layer = 8;
    }
}
