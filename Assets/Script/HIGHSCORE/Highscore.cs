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

public class Highscore : MonoBehaviour
{
    public int score;
    public string playerName;
    public TextMeshProUGUI highscoreText;
    [SerializeField]
    private List<HighscoreEntry> highscores;
    private string filePath;
    private const int maxHighscores = 10;
    HighscoreEntry lastEntry;
    int position;


    void Start()
    {
        highscoreText = GetComponent<TextMeshProUGUI>();
        filePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        LoadHighscores();
        UpdateHighscoreText();
    }

    public void AddHighscore()
    {
        playerName = GameManager.Instance.m_PlayerName;
        score = GameManager.Instance.m_Score;
        CheckEntry();
        SaveHighscores();
        UpdateHighscoreText();
        //make a new save data
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
        UpdateHighscoreText();
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
        UpdateHighscoreText();
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

    void UpdateHighscoreText()
    {
        int count = 0; ;
        highscoreText.text = "Highscores:\n";
        foreach (HighscoreEntry entry in highscores)
        {
            if (count < maxHighscores)
            {
                if (count < 5)
                {
                    highscoreText.text += $"<b>{count + 1}. {entry.playerName} - {entry.score}\n</b>";
                }
                else
                {
                    highscoreText.text += $"{count + 1}. {entry.playerName} - {entry.score}\n";
                }
                count++;
            }
            else
                break;
        }
        if (position > 10)
        {
             highscoreText.text += $"\nYour position: {position}. {lastEntry.playerName} - {lastEntry.score}\n";
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