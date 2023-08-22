using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Transform spawnPoint;
    private Character character;

    private float dieTime = 3f;

    private void Start()
    {
        character = GetComponent<Character>();
    }

    private void FixedUpdate()
    {
        if(character.isDead)
        {
            if (dieTime > 0f)
            {
                character.isPerformingAnAction = true;
                dieTime -= Time.deltaTime;
                return;
            }

            dieTime = 3f;

            character.health = character.maxHealth;
            transform.position = new Vector3(spawnPoint.position.x, transform.position.y, spawnPoint.position.z);
            transform.rotation = spawnPoint.rotation;
            character.isPerformingAnAction = false;
            character.isMovementRestriced = false;
            character.isImmuneToDamage = false;
            character.isLocking = false;
            character.isRunning = false;
            character.isAttacking = false;
            character.isDodging = false;
            character.isUILocked = false;
            character.isWeaponColliderActive = false;
            character.isStaggered = false;
            character.isDead = false;
            character.animator.SetFloat(character.animKeys.hitID, 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
            spawnPoint = other.transform;
    }

}
