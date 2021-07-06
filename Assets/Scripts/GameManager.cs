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
    public GameObject highScoreScreen;

    public Button startButton;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI weaponOneText;
    public TextMeshProUGUI weaponTwoText;
    public TextMeshProUGUI weaponThreeText;
    public TextMeshProUGUI finalWaveText;

    public TextMeshProUGUI highScoresNames;
    public TextMeshProUGUI highScoresValues;

    public TextMeshProUGUI invalidNameText;
    public TMP_InputField highScoreName;

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

    public string SelectUserName()
    {
        return name;
    }

    public void SaveScore()
    {
        if (highScoreName.text != "")
        {
            HighScores.Instance.UpdateHighScores(highScoreName.text, spawnManagerScript.waveNumber);
            invalidNameText.gameObject.SetActive(false);
            RestartGame();
        }
        else
        {
            invalidNameText.gameObject.SetActive(true);
        }
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
        Debug.Log("huh?");
        spawnManagerScript.IncrementWave();
        gameActiveUI.gameObject.SetActive(false);
        betweenRoundsScreen.gameObject.SetActive(true);
        waveText.text = ("Wave " + (spawnManagerScript.waveNumber-1) + " Completed!");
        isActive = false;
    }

    // Transitions from title screen to high score screen
    public void TitleToHighScore()
    {
        titleScreen.gameObject.SetActive(false);
        highScoreScreen.gameObject.SetActive(true);
        GenerateHighScores();
    }

    // Transitions from high score screen to title screen
    public void HighScoreToTitle()
    {
        highScoreScreen.gameObject.SetActive(false);
        titleScreen.gameObject.SetActive(true);
    }

    public void GenerateHighScores()
    {
        HighScores.Instance.LoadHighScore();
        int[] highScoreInts = new int[HighScores.numOfScores];
        highScoreInts = HighScores.Instance.GetScoreValues();

        string[] highScoreStrings = new string[HighScores.numOfScores];
        highScoreStrings = HighScores.Instance.GetScoreNames();

        string values = "Wave:\n\n";
        string names = "Name:\n\n";

        for (int i = 0; i < HighScores.numOfScores; i++)
        {
            names += highScoreStrings[i] + "\n";
            if (highScoreInts[i] != 0)
            {
                values += highScoreInts[i] + "\n";
            }
        }
        highScoresNames.text = names;
        highScoresValues.text = values;
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
