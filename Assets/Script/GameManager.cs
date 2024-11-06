using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    public int m_Score = 0;
    [SerializeField]
    public int m_Wave = 0;
    [HideInInspector]
    public string m_PlayerName = "Player";
    [SerializeField]
    [CanBeNull]
    private TMP_InputField m_InputField;
    private GameObject m_PauseMenu;
    private GameObject m_GameOverMenu;


    void Start()
    {
        m_PauseMenu = Resources.Load<GameObject>("PauseMenu");
        m_GameOverMenu = Resources.Load<GameObject>("GameOverMenu");
        m_InputField = GameObject.FindObjectOfType<TMP_InputField>();
    }

    public void SetName(string playerName)
    {
        m_PlayerName = playerName;
        Debug.Log("Player Name: " + m_PlayerName);
    }
    [ContextMenu("Next Scene")]
    private void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        AudioManager.Instance.StopMusic();

        if(m_PlayerName == "")
        {
            m_PlayerName = "Player";
        }
        HighscoreManager.Instance.AddHighscore();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y), new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).y));
    }
}
