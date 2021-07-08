using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStandard : EnemyRanged //INHERITANCE
{
    
    // Standard enemy shoots pistol
    protected override void Attack()
    {
        Instantiate(bulletPrefab, transform.position, transform.rotation, GameObject.Find("Bullets").transform);
        attackReady = false;
        StartCoroutine(AttackCooldown(attackCooldownTime));
    }

    // Set variables and call base activate
    protected override void Activate()
    {
        health = 3;
        speed = 3f;
        attackCooldownTime = 1.5f;
        base.Activate();
        preferredDistance = Random.Range(8, 18);
    }
}