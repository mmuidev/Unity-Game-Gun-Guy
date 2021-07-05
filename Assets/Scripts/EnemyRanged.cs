using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyRanged : Enemy
{
    [SerializeField] protected GameObject bulletPrefab;
    protected bool attackReady = false;
    
    // Movement variables
    protected bool isStrafing = false;
    protected bool isChangingDistance = false;

    protected int randStrafeRoll;
    protected int randDistanceRoll;

    protected float attackCooldownTime; // Needs to be set by child
    protected float preferredDistance; // Needs to be set by child
    private IEnumerator gapClose;

    // Start is called before the first frame update
    protected void Start()
    {
        Activate();
    }

    // Update is called once per frame
    protected void Update()
    {
        LookAtPlayer();

        // Shoot every x seconds
        if (attackReady)
        {
            Attack();
        }
        Move();
    }

    // Performs necessary actions when first generated
    // Child should call base and declare health, speed, and generate a preferred distance
    protected virtual void Activate() 
    { 
        player = GameObject.Find("Player");

        randStrafeRoll = Random.Range(1, 101); // Random integer from 1-100, Random.Range is not top-end inclusive for integers
        randDistanceRoll = Random.Range(1, 101); // Random integer from 1-100, Random.Range is not top-end inclusive for integers

        StartCoroutine(AttackCooldown(Random.Range(1.0f, 2.0f)));
    }

    // Turn to face player
    protected void LookAtPlayer()
    {
        var lookPos = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z) - new Vector3(transform.position.x, 0.0f, transform.position.z);
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f);
    }

    // Sets attackReady to false, waits certain amount of time, then sets to true
    protected IEnumerator AttackCooldown(float secondsToWait)
    {
        attackReady = false;
        yield return new WaitForSeconds(secondsToWait);
        attackReady = true;
    }

    // When colliding with a trigger
    protected void OnTriggerEnter(Collider other)
    {
        HitTaken(other);
    }

    // Check if GameObject is a bullet. If so, take damage based on type
    protected void HitTaken(Collider other)
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

    // Roll to see if enemy should strafe, and in which direction
    protected Vector3 StrafeCalculate()
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
    protected Vector3 DistanceCalculate()
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

    // Generate new random values for movement methods
    protected IEnumerator MoveTimer(float strafeTime, int moveType, int botLuck, int topLuck)
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

    // Moves enemy
    protected override void Move()
    {
        transform.Translate(Vector3.Normalize(DistanceCalculate() + StrafeCalculate()) * Time.deltaTime * speed);
    }

    // assign attacks based on enemy
    protected abstract void Attack();
}
