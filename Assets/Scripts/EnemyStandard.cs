using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStandard : MonoBehaviour
{
    private GameObject player;
    public GameObject bulletPrefab;
    private bool shotReady = false;
    private float shotCooldown = 2.0f;
    private float health = 3;
    private float speed = 3f;

    // Movement variables
    private bool isStrafing = false;
    private bool isChangingDistance = false;

    private int randStrafeRoll;
    private int randDistanceRoll;

    private float preferredDistance;

    public float upperZBound = 10.8f;
    public float xBound = 18.8f;
    public float lowerZBound = -7.4f;

    private IEnumerator gapClose;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

        preferredDistance = Random.Range(8, 18);

        randStrafeRoll = Random.Range(1, 101); // Random integer from 1-100, Random.Range is not top-end inclusive for integers
        randDistanceRoll = Random.Range(1, 101); // Random integer from 1-100, Random.Range is not top-end inclusive for integers

        StartCoroutine(BulletCooldown(Random.Range(1.0f, 2.0f)));
    }

    // Update is called once per frame
    void Update()
    {
        LookAtPlayer();

        // Shoot every x seconds
        if (shotReady)
        {
            ShootBullet();
        }

        MoveEnemy();
    }

    // Updates every frame immediately after Update() method
    private void LateUpdate()
    {
        StayInBoundary();
    }

    // Shoots a bullet in direction enemy is facing
    void ShootBullet()
    {        
        Instantiate(bulletPrefab, transform.position, transform.rotation, GameObject.Find("Bullets").transform);
        shotReady = false;
        StartCoroutine(BulletCooldown(shotCooldown));
    }

    // Waits a certain amount of time before firing next shot
    IEnumerator BulletCooldown(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        shotReady = true;
    }


    // If hit by player projectile, reduce health and check if dead
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerPistolProjectile"))
        {
            health -= 1.8f;
            Destroy(other.gameObject);
            CheckIsDead();
        }
        else if (other.gameObject.CompareTag("PlayerShotgunProjectile"))
        {
            health -= 1;
            Destroy(other.gameObject);
            CheckIsDead();
        }
        else if (other.gameObject.CompareTag("PlayerRifleProjectile"))
        {
            health -= 0.45f;
            Destroy(other.gameObject);
            CheckIsDead();
        }
    }


    // If dead, despawn
    private void CheckIsDead()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }


    // Calculate whether or not enemy should strafe
    private Vector3 StrafeCalculate()
    {
        // If timer not running, roll new random value
        if (!isStrafing)
        {
            float topRoll = 4.0f;
            float bottomRoll = 1.0f;
            isStrafing = true;
            StartCoroutine(MoveTimer(Random.Range(bottomRoll, topRoll), 1, 1, 101));
        }

        // Depending on what is rolled, move or dont move
        if (randStrafeRoll < 21)
        {
            return Vector3.right;
        }

        else if (randStrafeRoll < 41)
        {
            return -Vector3.right;
        }

        return Vector3.zero;
    }


    // Calculate whether or not enemy should close/raise distance between them and player
    private Vector3 DistanceCalculate()
    {
        // If timer not running, roll new random value
        if (!isChangingDistance)
        {
            float topRoll = 4.0f;
            float bottomRoll = 1.0f;

            isChangingDistance = true;

            // We put this coroutine in a variable because we need to stop it manually later
            gapClose = MoveTimer(Random.Range(bottomRoll, topRoll), 2, 1, 101);
            StartCoroutine(gapClose); 
        }

        // If roll is high enough, change distance
        if (randDistanceRoll < 61)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x, 2) + Mathf.Pow(player.transform.position.z - transform.position.z, 2));

            if (distance > preferredDistance + 1.0f)
            {
                return Vector3.forward;
            }

            else if (distance < preferredDistance - 1.0f)
            {
                return -Vector3.forward;
            }     

            // If distance is 0, roll to see if enemy should pause for a second or not
            else if (distance < preferredDistance + 0.005f && distance > preferredDistance - 0.005f)
            {
                float topRoll = 2.0f;
                float bottomRoll = 1.0f;

                StopCoroutine(gapClose); // Stop previous coroutine so it doesnt overwrite the next
                gapClose = MoveTimer(Random.Range(bottomRoll, topRoll), 2, 21, 101);

                isChangingDistance = true;
                StartCoroutine(gapClose);
            }

        }
        return Vector3.zero;
    }


    // Move enemy based on calculations from the other movement methods
    private void MoveEnemy()
    {
        transform.Translate(Vector3.Normalize(DistanceCalculate() + StrafeCalculate()) * Time.deltaTime * speed);
    }


    // Generate new random values for movement methods
    IEnumerator MoveTimer(float strafeTime, int moveType, int botLuck, int topLuck)
    {

        if (moveType == 1)
        {
            randStrafeRoll = Random.Range(botLuck, topLuck);
            yield return new WaitForSeconds(strafeTime);

            isStrafing = false;
        }
        else if (moveType == 2)
        {
            randDistanceRoll = Random.Range(botLuck, topLuck);
            yield return new WaitForSeconds(strafeTime);

            isChangingDistance = false;
        }
    }


    // Force enemy to stay within game boundary
    private void StayInBoundary()
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


    // Rotate front towards player
    private void LookAtPlayer()
    {
        var lookPos = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z) - new Vector3(transform.position.x, 0.0f, transform.position.z);
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
    }

}


