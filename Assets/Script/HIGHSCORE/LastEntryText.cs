using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LastEntryText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_Text;
    private HighscoreEntry lastEntry;
    private int position;

    void Start()
    {
        lastEntry = HighscoreManager.Instance.lastEntry;
        position = HighscoreManager.Instance.position;
        m_Text = GetComponent<TextMeshProUGUI>();
        m_Text.text = $"<b>{position + 1}. {lastEntry.playerName} - {lastEntry.score}\n</b>";
    }
}
