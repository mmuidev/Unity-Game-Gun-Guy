using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject spawnManager;
    public GameObject player;
    public GameObject activeGameElements;
    public GameObject gameActiveUI;
    public GameObject gameOverScreen;
    public GameObject betweenRoundsScreen;

    public Button startButton;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI weaponOneText;
    public TextMeshProUGUI weaponTwoText;
    public TextMeshProUGUI weaponThreeText;
    public TextMeshProUGUI finalWaveText;

    PlayerController playerControllerScript;
    SpawnManager spawnManagerScript;

    public bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        spawnManagerScript = spawnManager.GetComponent<SpawnManager>();
    }

    // Activates all elements needed to start the game
    public void startGame()
    {
        player.gameObject.SetActive(true);
        playerControllerScript = player.GetComponent<PlayerController>();
        titleScreen.gameObject.SetActive(false);
        updateHealth();
        gameActiveUI.gameObject.SetActive(true);
        StartCoroutine(delaySpawn());
        isActive = true;
    }


    // Delays activation of spawnManager
    IEnumerator delaySpawn()
    {
        yield return new WaitForSeconds(1);
        spawnManager.gameObject.SetActive(true);
        spawnManagerScript.StartSpawning();
    }


    // Updates player's health
    public void updateHealth()
    {
        healthText.text = ("HP: " + playerControllerScript.health);
        if (playerControllerScript.health < playerControllerScript.maxHealth/2)
        {
            healthText.color = new Color32(255, 100, 100, 255);
        }
    }


    // If game is lost, display game over screen
    public void loseGame()
    {
        activeGameElements.gameObject.SetActive(false);
        gameActiveUI.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        finalWaveText.text = ("You made it to wave " + spawnManagerScript.waveNumber);
    }


    // Restarts scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Starts next wave
    public void StartNewWave()
    {
        //activeGameElements.gameObject.SetActive(true); // SUPER BUGGED. PLAYER CANT SHOOT IF IT DEACTIVATES BETWEEN ROUNDS. SOLVE FIRST THEN ENABLE
        gameActiveUI.gameObject.SetActive(true);
        betweenRoundsScreen.gameObject.SetActive(false);
        spawnManagerScript.StartSpawning();
        isActive = true;
        playerControllerScript.health = playerControllerScript.maxHealth;
        updateHealth();
        healthText.color = new Color32(255, 255, 255, 255);
    }


    // Display between rounds screen
    public void BetweenRounds()
    {
        //activeGameElements.gameObject.SetActive(false);
        gameActiveUI.gameObject.SetActive(false);
        betweenRoundsScreen.gameObject.SetActive(true);
        waveText.text = ("Wave " + (spawnManagerScript.waveNumber-1) + " Completed!");
        isActive = false;
    }


    // Change colour of UI according to weapon selected
    public void changeColour()
    {
        if (playerControllerScript.weaponSelected == 0)
        {
            weaponOneText.color = new Color32(255, 255, 0, 255);
            weaponTwoText.color = new Color32(255, 255, 255, 255);
            weaponThreeText.color = new Color32(255, 255, 255, 255);
        }
        else if (playerControllerScript.weaponSelected == 1)
        {
            weaponOneText.color = new Color32(255, 255, 255, 255);
            weaponTwoText.color = new Color32(255, 255, 0, 255);
            weaponThreeText.color = new Color32(255, 255, 255, 255);
        }
        else if (playerControllerScript.weaponSelected == 2)
        {
            weaponOneText.color = new Color32(255, 255, 255, 255);
            weaponTwoText.color = new Color32(255, 255, 255, 255);
            weaponThreeText.color = new Color32(255, 255, 0, 255);
        }
    }


}
