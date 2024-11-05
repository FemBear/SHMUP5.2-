using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    private InputField m_InputField;
    private int m_health;
    private GameObject m_PauseMenu;
    private GameObject m_GameOverMenu;
    private WaveManager m_WaveManager;
    private UIManager m_UIManager;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            m_WaveManager = GameObject.FindObjectOfType<WaveManager>();
            gameObject.SetActive(true);
        }
        else
        {
            if (m_WaveManager != null)
            {
                m_WaveManager.enabled = false;
            }
        }
        m_PauseMenu = Resources.Load<GameObject>("PauseMenu");
        m_GameOverMenu = Resources.Load<GameObject>("GameOverMenu");
        m_InputField = GameObject.FindObjectOfType<InputField>();
        if (m_InputField != null)
        {
            m_InputField.onEndEdit.AddListener(delegate { SetName(m_InputField.text); });
        }
        else
        {
            return;
        }
        if (m_PlayerName == "")
        AudioManager.Instance.SwitchMusic(SceneManager.GetActiveScene().name);
        m_UIManager = UIManager.Instance;
    }

    public void SetName(string playerName)
    {
        m_PlayerName = playerName;
    }

    public void PauzeGame()
    {
        m_UIManager.TogglePauseMenu();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        AudioManager.Instance.StopMusic();
        m_UIManager.ShowGameOverMenu();
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
