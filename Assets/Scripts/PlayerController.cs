using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Variable creation
    private float speed = 7.5f;
    //public float zBound = 10.8f;
    public float upperZBound = 10.8f;
    public float lowerZBound = -7.4f;
    public float xBound = 18.8f;

    public int maxHealth = 10;
    public int health;

    private float pelletAngleDiff = 4.0f;
    private float pelletAngleVariance = 2.0f;
    private int numExtraPellets = 8;

    private float rifleSpreadRange = 5.0f;

    private float horizontalInput;
    private float verticalInput;
    
    private Vector3 mousePos;

    private bool shotReady = true;
    private bool dashReady = true;

    public GameObject bulletPrefab;
    public GameObject bulletShotgunPrefab;
    public GameObject bulletRiflePrefab;

    private float pistolCooldown = 0.45f;
    private float shotgunCooldown = 1.3f;
    private float rifleCooldown = 0.08f;

    GameManager gameManagerScript;

    public int weaponSelected = 0;

    IEnumerator shotCooldown;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        health = maxHealth;
    }


    // Update is called once per frame
    void Update()
    {
        if (gameManagerScript.isActive)
        {
            MovePlayer();

            // If left mouse is held down and shot is ready, shoot
            SelectWeapon();
            ShootCurrentWeapon();
        }

    }


    // Called once per frame after Update()
    private void LateUpdate()
    {
        StayInBoundary();
    }


    // Move player and make them face towards mouse location
    void MovePlayer()
    { 
        // Get keyboard movement inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Create normalized vector for inputs to ensure no matter what, magnitude is 1
        Vector3 inputVector = Vector3.Normalize(Vector3.forward * verticalInput + Vector3.right * horizontalInput);

        // Translate player based on inputs
        transform.Translate(inputVector * Time.deltaTime * speed, Space.World);

        if (Input.GetKeyDown(KeyCode.LeftShift) && dashReady)
        {
            Dash();
        }

        // Find mouse position
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 
                                                              Camera.main.transform.position.y - transform.position.y));

        // Rotate to look at mouse position
        transform.LookAt(mousePos);
    }


    // Interaction when enemy projectile collides with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyProjectile"))
        {
            health -= 2;
            gameManagerScript.updateHealth();
            Destroy(other.gameObject);
            CheckIsDead();
        }
        else if (other.gameObject.CompareTag("EnemyShotgunProjectile"))
        {
            health -= 1;
            gameManagerScript.updateHealth();
            Destroy(other.gameObject);
            CheckIsDead();
        }
        else if (other.gameObject.CompareTag("EnemyMeleeWeapon"))
        {
            health -= 3;
            gameManagerScript.updateHealth();
            CheckIsDead();
        }
    }


    // Restrict player movement to inside arena
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


    // Check if player has died
    private void CheckIsDead()
    {
        if (health <= 0)
        {
            gameManagerScript.loseGame();
        }
    }


    // Shoots projectile from player position towards front of player
    void PistolShot()
    {
        Vector3 startPosition = transform.position + transform.forward;
        Instantiate(bulletPrefab, startPosition, transform.rotation, GameObject.Find("Bullets").transform);
        shotCooldown = BulletCooldown(pistolCooldown);
        StartCoroutine(shotCooldown);
    }


    // Waits a certain amount of time before firing next shot
    IEnumerator BulletCooldown(float delay)
    {
        shotReady = false;
        yield return new WaitForSeconds(delay);
        shotReady = true;
    }


    // Dash system for player
    private void Dash()
    {
        // take vector of current user input
        Vector3 inputVector = Vector3.Normalize(Vector3.forward * verticalInput + Vector3.right * horizontalInput);

        // activate dash if player is moving in a direction
        if (inputVector != Vector3.zero)
        {
            StartCoroutine(DashMove(inputVector));
            StartCoroutine(DashCooldown());
        }


    }


    // Translate player over certain amount of time in dash direction
    IEnumerator DashMove(Vector3 direction)
    {
        float timePassed = 0;
        while (timePassed < 0.15f)
        { 
            transform.Translate(direction * 20f * Time.deltaTime, Space.World);
            timePassed += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return 0;
    }


    // Cooldown for dash
    IEnumerator DashCooldown()
    {
        dashReady = false;
        yield return new WaitForSeconds(1.5f);
        dashReady = true;
    }


    /// Calculate how to shoot pellets from shotgun. Generate a random value for each pellet spawned to see
    /// how much of an angle they should spread out by.
    private void ShotgunShot()
    {
        float yDiffRight = Random.Range(pelletAngleDiff - pelletAngleVariance, pelletAngleDiff + pelletAngleVariance);
        float yDiffLeft = Random.Range(pelletAngleDiff - pelletAngleVariance, pelletAngleDiff + pelletAngleVariance);

        Quaternion myRotationRight;
        Quaternion myRotationLeft;

        Vector3 startPosition = transform.position + transform.forward;
        Instantiate(bulletShotgunPrefab, startPosition, transform.rotation, GameObject.Find("Bullets").transform);

        for (int i = 0; i < numExtraPellets / 2; i++)
        {
            myRotationRight = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + yDiffRight, transform.localEulerAngles.z);
            myRotationLeft = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y - yDiffLeft, transform.localEulerAngles.z);
            Instantiate(bulletShotgunPrefab, startPosition, myRotationRight, GameObject.Find("Bullets").transform);
            Instantiate(bulletShotgunPrefab, startPosition, myRotationLeft, GameObject.Find("Bullets").transform);
            Debug.Log(transform.rotation);
            yDiffRight += Random.Range(pelletAngleDiff - pelletAngleVariance, pelletAngleDiff + pelletAngleVariance);
            yDiffLeft += Random.Range(pelletAngleDiff - pelletAngleVariance, pelletAngleDiff + pelletAngleVariance);
        }
        shotCooldown = BulletCooldown(shotgunCooldown);
        StartCoroutine(shotCooldown);
    }

    
    // Takes user key input and selects weapon
    private void SelectWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponSelected != 0)
        {
            weaponSelected = 0;
            gameManagerScript.changeColour();
            StopCoroutine(shotCooldown);
            shotCooldown = BulletCooldown(0.4f);
            StartCoroutine(shotCooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && weaponSelected != 1)
        {
            weaponSelected = 1;
            gameManagerScript.changeColour();
            StopCoroutine(shotCooldown);
            shotCooldown = BulletCooldown(0.4f);
            StartCoroutine(shotCooldown);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && weaponSelected != 2)
        {
            weaponSelected = 2;
            gameManagerScript.changeColour();
            StopCoroutine(shotCooldown);
            shotCooldown = BulletCooldown(0.4f);
            StartCoroutine(shotCooldown);
        }
    }


    // Checks which weapon is selected and fires that weapon
    private void ShootCurrentWeapon()
    {
        if (Input.GetMouseButton(0) && shotReady)
        {
            if (weaponSelected == 0)
                PistolShot();
            else if (weaponSelected == 1)
                ShotgunShot();
            else if (weaponSelected == 2)
                RifleShot();
        }
    }


    // Fires bullet with properties of rifle
    private void RifleShot()
    {
        float randSpread = Random.Range(-rifleSpreadRange, rifleSpreadRange);
        Quaternion bulletAngle;

        bulletAngle = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + randSpread, transform.localEulerAngles.z);
        Instantiate(bulletRiflePrefab, transform.position, bulletAngle, GameObject.Find("Bullets").transform);

        shotCooldown = BulletCooldown(rifleCooldown);
        StartCoroutine(shotCooldown);
    }

}
