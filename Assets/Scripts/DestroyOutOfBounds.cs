using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOutOfBounds : MonoBehaviour
{
    // Boundaries
    private float verticalBound = 20.0f;
    private float horizontalBound = 30.0f;

    // Update is called once per frame
    void Update()
    {
        // Destroy game object if out of bounds
        if (transform.position.z > verticalBound || transform.position.z < -verticalBound)
        {
            Destroy(gameObject);
        }
        else if (transform.position.x > horizontalBound || transform.position.x < -horizontalBound)
        {
            Destroy(gameObject);
        }
    }
}
