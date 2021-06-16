using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DespawnAfterTime : MonoBehaviour
{
    public float topBound = .8f;
    public float botBound = .55f;
    private float despawnTime;


    // Start is called before the first frame update
    void Start()
    {
        despawnTime = Random.Range(botBound, topBound); // Generate random value between bounds as despawn time
        StartCoroutine(DespawnTimer());
    }

    // Destroy Game Object after x time
    IEnumerator DespawnTimer()
    {
        yield return new WaitForSeconds(despawnTime);
        Destroy(gameObject);
    }
}
