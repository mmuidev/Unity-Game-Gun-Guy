using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShotgun : EnemyRanged // INHERITANCE
{
    // Values for shotgun
    private float pelletAngleDiff = 8f; 
    private int numExtraPellets = 4;

    // Shotgun enemy shoots pellets from shotgun
    protected override void Attack() // ABSTRACTION AND POLYMORPHISM
    {
        float yDiff = pelletAngleDiff;

        Quaternion myRotationRight;
        Quaternion myRotationLeft;

        Vector3 startPosition = transform.position + transform.forward;
        Instantiate(bulletPrefab, startPosition, transform.rotation, GameObject.Find("Bullets").transform);

        for (int i = 0; i < numExtraPellets / 2; i++)
        {
            myRotationRight = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + yDiff, transform.localEulerAngles.z);
            myRotationLeft = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y - yDiff, transform.localEulerAngles.z);
            Instantiate(bulletPrefab, startPosition, myRotationRight, GameObject.Find("Bullets").transform);
            Instantiate(bulletPrefab, startPosition, myRotationLeft, GameObject.Find("Bullets").transform);
            yDiff += pelletAngleDiff;
        }
        StartCoroutine(AttackCooldown(attackCooldownTime));
    }

    // Set variables and call base activate
    protected override void Activate()
    {
        health = 4;
        speed = 4f;
        attackCooldownTime = 2.5f;
        base.Activate();
        preferredDistance = Random.Range(1, 4);
    }
}


