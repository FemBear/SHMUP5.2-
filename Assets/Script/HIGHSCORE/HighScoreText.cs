using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreText : MonoBehaviour
{
    private TextMeshProUGUI highscoreText;
    [SerializeField]
    private List<HighscoreEntry> highscores;
    private HighscoreEntry lastEntry;
    private int position;

    void Start()
    {
        highscoreText = GetComponent<TextMeshProUGUI>();
        highscores = HighscoreManager.Instance.GetHighscores();
        UpdateHighscoreText();
    }
    
    [ContextMenu("Update Highscore Text")]
    void UpdateHighscoreText()
    {
        int count = 0;
        lastEntry = HighscoreManager.Instance.lastEntry;
        position = HighscoreManager.Instance.position;
        highscoreText.text = "Highscores:\n";
        Debug.Log("Updating highscore text");

        foreach (HighscoreEntry entry in highscores)
        {
            Debug.Log($"Entry: {entry.playerName} - {entry.score}");
            if (count < HighscoreManager.maxHighscores)
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
            {
                break;
            }
        }
        if (HighscoreManager.Instance.position >= HighscoreManager.maxHighscores)
        {
            highscoreText.text += $"<b>{position + 1}. {lastEntry.playerName} - {lastEntry.score}\n</b>";
        }
        Debug.Log("Highscore text updated: " + highscoreText.text);
    }
}
