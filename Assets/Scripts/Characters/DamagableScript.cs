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

    private float guardThresholdAngle = 30f;
    private float parryThresholdAngle = 10f;

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
        float angularDifference;

        if (attackerCharacter.attackInfo.type == AttackType.SLASH_WEAPON_TOP)
            angularDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360f - attackerCharacter.attackInfo.topAngle));
        else
            angularDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360f - attackerCharacter.attackInfo.bottomAngle));

        if (angularDifference > guardThresholdAngle || attackerCharacter.attackInfo.type == AttackType.THRUST || character.isPerformingAnAction)
            ReceiveDamage(attackerCharacter);
        else if (angularDifference < parryThresholdAngle)
            Parry(attackerCharacter);
        else
            Guard(attackerCharacter, angularDifference);

    }

    public void ReceiveDamage(Character attackerCharacter)
    {
        character.health -= attackerCharacter.attackInfo.damageAmmount;
        print(gameObject.name + "'s remaining health: " + character.health);

        if (character.health <= 0f)
        {
            Die(attackerCharacter);
            return;
        }

        if(!character.isPerformingAnAction)
        {
            characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);
            float randomAnimID = Random.Range(1f, 5f);
            characterAnimator.SetFloat(character.animKeys.hitID, randomAnimID);
        } else if (character.isAttacking && character.attackInfo.type == AttackType.THRUST)
        {
            attackerCharacter.characterAnimator.SetFloat(attackerCharacter.animKeys.hitID, 10f);
            attackerCharacter.characterAnimator.SetTrigger(attackerCharacter.animKeys.hitTriggerKey);
        }
    }

    public void Guard(Character attackerCharacter, float angularDifference)
    {
        float mitigationMultiplier = angularDifference / guardThresholdAngle;
        
        float damage = attackerCharacter.attackInfo.damageAmmount * mitigationMultiplier / 1.5f;
        character.health -= damage;
        //print(gameObject.name + "'s remaining health: " + character.health);

        if (character.health <= 0f)
        {
            Die(attackerCharacter);
            return;
        }

        if (!character.isPerformingAnAction)
        {
            characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);
            float randomAnimID = Random.Range(6f, 8f);
            characterAnimator.SetFloat(character.animKeys.hitID, randomAnimID);
        }
    }

    public void Parry(Character attackerCharacter)
    {
        
        print("parry");
        characterAnimator.SetFloat(character.animKeys.hitID, 9f);
        characterAnimator.SetTrigger(character.animKeys.hitTriggerKey);

        attackerCharacter.characterAnimator.SetFloat(attackerCharacter.animKeys.hitID, 10f);
        attackerCharacter.characterAnimator.SetTrigger(attackerCharacter.animKeys.hitTriggerKey);
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
