using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.SearchService;
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

    void Start()
    {
        m_PauseMenu = Resources.Load<GameObject>("PauseMenu");
        m_InputField = GameObject.FindObjectOfType<InputField>();
        if (m_InputField != null)
        {
            m_InputField.onEndEdit.AddListener(delegate { SetName(); });
        }
        else
        {
            return;
        }
    }
    public void SetName()
    {
        m_PlayerName = m_InputField.text;
    }

    public void PauzeGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            Destroy(m_PauseMenu);
        }
        else
        {
            Time.timeScale = 0;
            Instantiate(m_PauseMenu);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y), new Vector2(Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).y));
    }
}
