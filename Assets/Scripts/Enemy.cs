using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected GameObject player;
    protected PlayerController playerControllerScript;

    protected float health; // Needs to be set by child
    protected float speed; // Needs to be set by child


    // Boundaries of the arena

    // StayInBoundary in LateUpdate to prevent glitch
    protected void LateUpdate()
    {
        StayInBoundary();
    }

    // check health. If health is below or equal 0, destroy GameObject
    protected void CheckIsDead()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    // force entity to stay within arena boundary
    protected virtual void StayInBoundary()
    {
        if (transform.position.z > playerControllerScript.UpperZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerControllerScript.UpperZBound);
        }
        if (transform.position.z < playerControllerScript.LowerZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, playerControllerScript.LowerZBound);
        }
        if (transform.position.x > playerControllerScript.XBound)
        {
            transform.position = new Vector3(playerControllerScript.XBound, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -playerControllerScript.XBound)
        {
            transform.position = new Vector3(-playerControllerScript.XBound, transform.position.y, transform.position.z);
        }
    }

    // assign individual movement code based on enemy
    protected abstract void Move();

    protected virtual void Activate()
    {
        player = GameObject.Find("Player");
        playerControllerScript = player.GetComponent<PlayerController>();
    }
}