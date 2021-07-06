using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class HighScores : MonoBehaviour
{
    public static HighScores Instance { get; private set; }

    public static int numOfScores = 10;
    
    private int[] scoreValues = new int[numOfScores];
    private string[] scoreNames = new string[numOfScores];

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [System.Serializable]
    class HighScoreData
    {
        public int[] scoreValues = new int[numOfScores];
        public string[] scoreNames = new string[numOfScores];
    }

    // read json, put high score integers into scores array
    public void UpdateHighScores(string newName, int newScore) 
    {
        LoadHighScore();
        for (int i = 0; i < numOfScores; i++)
        { 
            if (newScore > scoreValues[i])
            {
                int descendingCounter = numOfScores - 1;

                while (i < descendingCounter)
                {
                    scoreValues[descendingCounter] = scoreValues[descendingCounter - 1];
                    scoreNames[descendingCounter] = scoreNames[descendingCounter - 1];
                    descendingCounter--;

                    
                }

                scoreValues[i] = newScore;
                scoreNames[i] = newName;

                SaveHighScore();
                return;
            }
        }

    }

    public void SaveHighScore()
    {
        int counter = 0;
        HighScoreData data = new HighScoreData();
        while (counter < numOfScores && scoreValues[counter] != 0)
        {
            data.scoreValues[counter] = scoreValues[counter];
            data.scoreNames[counter] = scoreNames[counter];
            counter++;
        }

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/highscores.json", json);
        Debug.Log(Application.persistentDataPath);
    }

    public void LoadHighScore()
    {
        string path = Application.persistentDataPath + "/highscores.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            HighScoreData data = JsonUtility.FromJson<HighScoreData>(json);

            for (int i = 0; i < numOfScores; i++)
            {
                scoreValues[i] = data.scoreValues[i];
                scoreNames[i] = data.scoreNames[i];
            }
        }
    }

    public int[] GetScoreValues()
    {
        return scoreValues;
    }

    public string[] GetScoreNames()
    {
        return scoreNames;
    }
}

