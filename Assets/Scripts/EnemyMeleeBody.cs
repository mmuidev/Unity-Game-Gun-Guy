using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeBody : MonoBehaviour
{
    private GameObject enemyMelee;
    EnemyMelee enemyMeleeScript;

    // Start is called before the first frame update
    void Start()
    {
        enemyMelee = transform.parent.gameObject;
        enemyMeleeScript = enemyMelee.GetComponent<EnemyMelee>();
    }


    // If hit by trigger, take damage based on what it was
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerPistolProjectile"))
        {
            Destroy(other.gameObject);
            enemyMeleeScript.HitTaken(1.8f);
        }
        else if (other.gameObject.CompareTag("PlayerShotgunProjectile"))
        {
            Destroy(other.gameObject);
            enemyMeleeScript.HitTaken(1f);
        }
        else if (other.gameObject.CompareTag("PlayerRifleProjectile"))
        {
            Destroy(other.gameObject);
            enemyMeleeScript.HitTaken(0.45f);
        }
    }
}
