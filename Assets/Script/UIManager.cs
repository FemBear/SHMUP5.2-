using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{

    [SerializeField]
    private GameObject m_PauseMenu;
    [SerializeField]
    private GameObject m_GameOverMenu;
    [SerializeField]
    private InputField m_InputField;

    void Start()
    {
        m_PauseMenu = Resources.Load<GameObject>("PauseMenu");
        m_GameOverMenu = Resources.Load<GameObject>("GameOverMenu");
        m_InputField = GameObject.FindObjectOfType<InputField>();
        if (m_InputField != null)
        {
            m_InputField.onEndEdit.AddListener(delegate { GameManager.Instance.SetName(m_InputField.text); });
        }
    }
    public void TogglePauseMenu()
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

    public void ShowGameOverMenu()
    {
        Time.timeScale = 0;
        if (m_GameOverMenu != null)
        {
            Instantiate(m_GameOverMenu);
        }
    }
}
