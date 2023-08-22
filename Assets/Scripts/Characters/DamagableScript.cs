using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableScript : MonoBehaviour
{
    [SerializeField] private List<float> protectedAngles;
    
    // References
    private Character character;
    private WeaponDial weaponDial;

    // Variables
    private Character attackerCharacter;

    private float damageResetTime = 0.3f;
    private float damageResetTimer = 0f;

    private float guardThresholdAngle = 45f;
    private float parryThresholdAngle = 10f;

    private void Start()
    {
        character = GetComponentInChildren<Character>();
        weaponDial = GetComponent<WeaponDial>();

        damageResetTimer = damageResetTime;
    }

    private void Update()
    {
        // When taken damage, give invincibility for a short ammount of time
        if(damageResetTimer < damageResetTime)
            damageResetTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Weapon" && other.transform.root.gameObject != gameObject && !character.isImmuneToDamage && !character.isDead && damageResetTimer >= damageResetTime)
        {
            attackerCharacter = other.transform.root.gameObject.GetComponent<Character>();

            ManageDamage();

            // Resets
            damageResetTimer = 0f;
            attackerCharacter.ClearAttackInfo();
        }
    }

    #region MAIN
    
    public void ManageDamage()
    {
        // Check if any of the two Weapon Angles is inside the threshold
        float angularDifference;

        foreach (float protectedAngle in protectedAngles)
        {
            if (attackerCharacter.attackInfo.type == AttackType.SLASH_WEAPON_TOP)
                angularDifference = Mathf.Abs(Mathf.DeltaAngle(protectedAngle, attackerCharacter.attackInfo.topAngle));
            else
                angularDifference = Mathf.Abs(Mathf.DeltaAngle(protectedAngle, attackerCharacter.attackInfo.bottomAngle));

            // Depending on the angular difference, choose what to apply
            if (angularDifference < parryThresholdAngle && !character.isMovementRestriced && attackerCharacter.attackInfo.type != AttackType.THRUST)
            {
                Parry(false);
                return;
            }
        }
        
        if (attackerCharacter.attackInfo.type == AttackType.SLASH_WEAPON_TOP)
            angularDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360 - attackerCharacter.attackInfo.topAngle));
        else
            angularDifference = Mathf.Abs(Mathf.DeltaAngle(weaponDial.topAngle, 360 - attackerCharacter.attackInfo.bottomAngle));

        // Depending on the angular difference, choose what to apply
        if (angularDifference > guardThresholdAngle || attackerCharacter.attackInfo.type == AttackType.THRUST || character.isPerformingAnAction)
            Hit();
        else if (angularDifference < parryThresholdAngle && !character.isMovementRestriced)
            Parry();
        else if (!character.isMovementRestriced)
            Guard(angularDifference);
    }

    #endregion

    #region HELPERS
    public void Hit()
    {
        // Trigger Counter Parry to attacker
        if (character.isAttacking && character.attackInfo.type == AttackType.THRUST && attackerCharacter.attackInfo.type == AttackType.THRUST)
        {
            attackerCharacter.animator.SetFloat(attackerCharacter.animKeys.hitID, 10f);
            attackerCharacter.animator.SetTrigger(attackerCharacter.animKeys.hitTriggerKey);
            return;
        }

        // Apply Damage
        character.health -= attackerCharacter.attackInfo.damageAmmount;

        // Check Death
        if (character.health <= 0f)
        {
            Die();
            return;
        }

        // Apply/Not apply staggered animation
        if (!character.isWeaponColliderActive)
        {
            float randomAnimID = Random.Range(1f, 5f);
            character.animator.SetFloat(character.animKeys.hitID, randomAnimID);
            character.animator.SetTrigger(character.animKeys.hitTriggerKey);
        }
    }

    public void Guard(float angularDifference)
    {
        // Calculate damage received based on accuracy && apply damage
        float mitigationMultiplier = parryThresholdAngle / angularDifference;
        float mitigationAmmount = 4f * mitigationMultiplier * mitigationMultiplier;
        float damage = attackerCharacter.attackInfo.damageAmmount / (1f + mitigationAmmount);
        character.health -= damage;

        // Check Death
        if (character.health <= 0f)
        {
            Die();
            return;
        }

        // Update UI
        weaponDial.SetGuardSprites();

        // Apply/Not apply staggered animation
        if (!character.isPerformingAnAction)
        {
            character.animator.SetTrigger(character.animKeys.hitTriggerKey);
            float randomAnimID = Random.Range(6f, 8f);
            character.animator.SetFloat(character.animKeys.hitID, randomAnimID);
        }
    }

    public void Parry(bool triggerCounteranimation = true)
    {
        // Parry animation
        if(triggerCounteranimation)
        {
            character.animator.SetFloat(character.animKeys.hitID, 9f);
            character.animator.SetTrigger(character.animKeys.hitTriggerKey);
        }

        // Update UI
        WeaponDial attackerWeaponDial = attackerCharacter.gameObject.GetComponent<WeaponDial>();
        attackerWeaponDial.SetCancelledSprites();

        // Trigger Counter Parry to attacker
        attackerCharacter.animator.SetFloat(attackerCharacter.animKeys.hitID, 10f);
        attackerCharacter.animator.SetTrigger(attackerCharacter.animKeys.hitTriggerKey);
    }

    public void Die()
    {
        // Make attacker stop locking
        attackerCharacter.isLocking = false;

        // Update UI
        weaponDial.SetCancelledSprites();

        // Trigger Death animation
        float randomAnimID = Mathf.Round(Random.Range(11f, 12f));
        character.animator.SetFloat(character.animKeys.hitID, randomAnimID);
        character.animator.SetTrigger(character.animKeys.hitTriggerKey);

        // Update tag & layer
        gameObject.tag = "Corpse";
        gameObject.layer = 8;
    }

    #endregion
}
