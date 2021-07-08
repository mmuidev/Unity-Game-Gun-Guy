using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private GameObject player;
    public GameObject enemyStandardPrefab;
    public GameObject enemyMeleePrefab;
    public GameObject enemyShotgunPrefab;

    public int waveNumber = 1;

    GameManager gameManagerScript;

    // How many enemies to spawn each wave
    private int initialSpawn = 3;
    private int totalSpawn = 5;
    private int enemyCount;
    private int enemyStartSpawnNum = 2;
    private int tooFewEnemies = 1;

    private float spawnLuckOffset = 0;

    private bool isSpawning = false;
    private bool inBetweenRounds = false;

    PlayerController playerControllerScript;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();

        playerControllerScript = player.GetComponent<PlayerController>();
    }


    // Update is called once per frame
    void Update()
    {
        // Check how many enemies are remaining to see when to end wave
        CheckWaveStatus();
    }


    // Starts wave by spawning enemies
    IEnumerator StartWave()
    {
        yield return new WaitForSeconds(0.5f);

        // Spawn initial bunch of enemies
        for (int i = 0; i < initialSpawn; i++)
        {
            SpawnEnemy();
        }

        // Let rest of enemies trickle in over time
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < totalSpawn - initialSpawn; i++)
        {
            while (enemyCount >= enemyStartSpawnNum)
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.5f);
            if (enemyCount <= tooFewEnemies)
            {
                yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
                SpawnEnemy();
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(1f, 2.5f));
                SpawnEnemy();
            }
        }

        // Increase wave number and difficulty
        isSpawning = false;

    }


    // Spawn enemy in random location
    void SpawnEnemy()
    {
        float whichEnemy = Random.Range(0, 100f - spawnLuckOffset);
        if (whichEnemy > 50)
        {
            var newEnemy = Instantiate(enemyStandardPrefab, GenerateSpawnPosition(), enemyStandardPrefab.transform.rotation, GameObject.Find("Enemies").transform);
            spawnLuckOffset += 15; // Ensures there aren't too many standard enemies being spawned

        }
        else if (whichEnemy > 30)
        {
            var newEnemy = Instantiate(enemyShotgunPrefab, GenerateSpawnPosition(), enemyStandardPrefab.transform.rotation, GameObject.Find("Enemies").transform);
            spawnLuckOffset = 0;
        }
        else
        {
            var newEnemy = Instantiate(enemyMeleePrefab, GenerateSpawnPosition(), enemyStandardPrefab.transform.rotation, GameObject.Find("Enemies").transform);
            spawnLuckOffset = 0;
        }
    }


    // Generate random position within arena to spawn enemy
    Vector3 GenerateSpawnPosition()
    {
        bool nearPlayer = true;
        Vector3 spawnPos = Vector3.forward;

        // Generate new positions until one is found that isn't near the player
        while (nearPlayer)
        {
            float xPos = Random.Range(-playerControllerScript.XBound, playerControllerScript.XBound);
            float zPos = Random.Range(playerControllerScript.LowerZBound, playerControllerScript.UpperZBound);
            nearPlayer = false;

            // Compare position generated to player position. If too close, loop again
            if (xPos < player.transform.position.x + 5 && xPos > player.transform.position.x -5
                && zPos < player.transform.position.z + 5 && zPos > player.transform.position.z -5)
            {
                nearPlayer = true;
            }

            spawnPos = new Vector3(xPos, 1, zPos);
        }

        return spawnPos;
    }


    // Checks if wave is over
    private void CheckWaveStatus()
    {
        enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (enemyCount == 0 && !isSpawning && !inBetweenRounds)
        {
            gameManagerScript.BetweenRounds();
            inBetweenRounds = true;
        }
    }


    // Start spawning wave
    public void StartSpawning()
    {
        isSpawning = true;
        inBetweenRounds = false;
        StartCoroutine(StartWave());
    }

    public void IncrementWave()
    {
        waveNumber++;
        initialSpawn = 3 + waveNumber / 3;
        totalSpawn += 3;
        enemyStartSpawnNum = initialSpawn - 1;
        tooFewEnemies = 1 + enemyStartSpawnNum / 3;
    }
}
