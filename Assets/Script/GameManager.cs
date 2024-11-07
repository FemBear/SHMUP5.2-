using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    #region Variables
    [SerializeField]
    public int m_Score = 0;
    [SerializeField]
    public int m_Wave = 0;
    [HideInInspector]
    public string m_PlayerName = "Player";

    public int m_ComboMultiplier = 1;
    private float m_ComboTimer = 0f;
    private float m_ComboDuration = 5f;
    #endregion

    #region Basics
    private void Update()
    {
        if (m_ComboTimer > 0)
        {
            m_ComboTimer -= Time.deltaTime;
            if (m_ComboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }
    #endregion

    #region Highscore/Combo
    public void AddScore(int score)
    {
        m_Score += score * m_ComboMultiplier;
        m_ComboMultiplier++;
        m_ComboTimer = m_ComboDuration;
        UIManager.Instance.UpdateScoreAndComboUI(m_Score, m_ComboMultiplier);
    }
    public void SetName(string playerName)
    {
        m_PlayerName = playerName;
        Debug.Log("Player Name: " + m_PlayerName);
        UIManager.Instance.m_InputField.text = "Name Set";
        NextScene();
    }

    private void ResetCombo()
    {
        m_ComboMultiplier = 1;
        UIManager.Instance.UpdateScoreAndComboUI(m_Score, m_ComboMultiplier);
    }
    #endregion

    #region Utility/Setup
    private void OnApplicationQuit()
    {
        HighscoreManager.Instance.SaveHighscores();
    }

    [ContextMenu("Next Scene")]
    private void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GameOver()
    {
        AudioManager.Instance.StopMusic();

        if(m_PlayerName == "")
        {
            m_PlayerName = "Player";
        }
        HighscoreManager.Instance.AddHighscore();
        UIManager.Instance.ShowGameOverMenu();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y), new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).y));
    }
    #endregion
}
