using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected GameObject player;

    protected float health; // Needs to be set by child
    protected float speed; // Needs to be set by child


    // Boundaries of the arena
    protected float upperZBound = 10.8f;
    protected float lowerZBound = -7.4f;
    protected float xBound = 18.8f;

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
        if (transform.position.z > upperZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, upperZBound);
        }
        if (transform.position.z < lowerZBound)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, lowerZBound);
        }
        if (transform.position.x > xBound)
        {
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -xBound)
        {
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        }
    }

    // assign individual movement code based on enemy
    protected abstract void Move();
}