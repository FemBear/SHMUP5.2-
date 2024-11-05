using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.IO;
using Random = UnityEngine.Random;
[System.Serializable]
public class HighscoreEntry
{
    public int score;
    public string playerName;

    public HighscoreEntry(int score, string playerName)
    {
        this.score = score;
        this.playerName = playerName;
    }
}

public class HighscoreManager : Singleton<HighscoreManager>
{
    public int score;
    public string playerName;
    [SerializeField]
    public List<HighscoreEntry> highscores;
    public string filePath;
    public const int maxHighscores = 10;
    public HighscoreEntry lastEntry;
    public int position;


    new void Awake()
    {
        base.Awake();
        filePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        LoadHighscores();
    }

    public void AddHighscore()
    {
        playerName = GameManager.Instance.m_PlayerName;
        score = GameManager.Instance.m_Score;
        CheckEntry();
        SaveHighscores();
    }

    [ContextMenu("Add Test Highscore")]
    public void AddTestHighscore()
    {
        playerName = "Test";
        score = Random.Range(1, 10000000);
        for (int i = 0; i < 100; i++)
        {
            highscores.Add(new HighscoreEntry(Random.Range(1, 10000000), "Test" + i));
        }
        CheckEntry();
        SaveHighscores();
    }

    void SaveHighscores()
    {
        HighscoreList highscoreList = new HighscoreList(highscores);
        string json = JsonUtility.ToJson(highscoreList);
        File.WriteAllText(filePath, json);
    }

    [ContextMenu("Reset Highscores")]
    void ResetHighscores()
    {
        highscores = new List<HighscoreEntry>();
        lastEntry = null;
        SaveHighscores();
    }

    void LoadHighscores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            highscores = JsonUtility.FromJson<HighscoreList>(json).highscores;
        }
        else
        {
            highscores = new List<HighscoreEntry>();
        }
    }


    void CheckEntry()
    {
        if (highscores.Exists(x => x.playerName == playerName))
        {
            HighscoreEntry entry = highscores.Find(x => x.playerName == playerName);
            if (score > entry.score)
            {
                entry.score = score;
            }
        }
        else
        {
            highscores.Add(new HighscoreEntry(score, playerName));
        }
        lastEntry = highscores.LastOrDefault();
        Debug.Log("before sort" + position);
        highscores = highscores.OrderByDescending(x => x.score).ToList();
        position = highscores.IndexOf(lastEntry);
        Debug.Log("after sort" + position);
        SaveHighscores();
    }

    public List<HighscoreEntry> GetHighscores()
    {
        return highscores;
    }

    [System.Serializable]
    private class HighscoreList
    {
        public List<HighscoreEntry> highscores;

        public HighscoreList(List<HighscoreEntry> highscores)
        {
            this.highscores = highscores;
        }
    }
}