using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : Enemy // INHERITANCE
{
    private bool isCharging = false;
    private bool chargePrepare = false;

    // Start is called before the first frame update
    protected void Start()
    {
        Activate();
    }


    // Update is called once per frame
    protected void Update()
    {
        Move();

    }

    // Wait a second before allowing charge again
    IEnumerator chargeCooldown()
    {
        yield return new WaitForSeconds(1);
        isCharging = true;
        chargePrepare = false;
    }

    // Reduce health by damage amount
    public void HitTaken(float damage)
    {
        health -= damage;
        CheckIsDead();
    }


    // Calculates how to move
    protected override void Move()
    {
        // Calculate player position to look at player
        var lookPos = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z) - new Vector3(transform.position.x, 0.0f, transform.position.z);
        var rotation = Quaternion.LookRotation(lookPos);

        if (isCharging) // Charge towards player position, small angle turn
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 45f * Time.deltaTime);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
        else if (!chargePrepare)
        {
            StartCoroutine(chargeCooldown());
            chargePrepare = true;
        }
        else // When preparing charge, turn faster
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 200f * Time.deltaTime);
        }
    }

    protected override void Activate()
    {
        base.Activate();

        health = 5;
        speed = 15f;
    }

    // Ensure enemy does not walk outside of boundary
    protected override void StayInBoundary()
    {
        if (transform.position.z > playerControllerScript.UpperZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerControllerScript.UpperZBound);
            isCharging = false;
        }
        if (transform.position.z < playerControllerScript.LowerZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerControllerScript.LowerZBound);
            isCharging = false;
        }
        if (transform.position.x > playerControllerScript.XBound)
        {
            transform.position = new Vector3(playerControllerScript.XBound, transform.position.y, transform.position.z);
            isCharging = false;
        }
        if (transform.position.x < -playerControllerScript.XBound)
        {
            transform.position = new Vector3(-playerControllerScript.XBound, transform.position.y, transform.position.z);
            isCharging = false;
        }
    }
}
