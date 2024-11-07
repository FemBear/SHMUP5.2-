using System.Collections;
using JetBrains.Annotations;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Variables
    [SerializeField]
    public Canvas m_Canvas;
    [CanBeNull]
    [SerializeField]
    private GameObject m_PauseMenu;
    [CanBeNull]
    [SerializeField]
    private GameObject m_GameOverMenu;
    [CanBeNull]
    [SerializeField]
    public TMP_InputField m_InputField;
    [CanBeNull]
    [SerializeField]
    private Image[] m_HealthImage;
    [CanBeNull]
    [SerializeField]
    private Slider m_FuelBar;
    [CanBeNull]
    [SerializeField]
    private TextMeshProUGUI m_ScoreText;
    private PlayerControls m_InputControls;
    public GameObject m_FirstSelected;
    #endregion

    #region Basics
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        CheckForController();
    }
    #endregion

    #region UI
    public void UpdateHealthUI(int health)
    {
        if (m_HealthImage.Length > 0)
        {
            switch (health)
            {
                case 0:
                    m_HealthImage[0].enabled = false;
                    m_HealthImage[1].enabled = false;
                    m_HealthImage[2].enabled = false;
                    break;
                case 1:
                    m_HealthImage[0].enabled = true;
                    m_HealthImage[1].enabled = false;
                    m_HealthImage[2].enabled = false;
                    break;
                case 2:
                    m_HealthImage[0].enabled = true;
                    m_HealthImage[1].enabled = true;
                    m_HealthImage[2].enabled = false;
                    break;
                case 3:
                    m_HealthImage[0].enabled = true;
                    m_HealthImage[1].enabled = true;
                    m_HealthImage[2].enabled = true;
                    break;
            }
        }
        else
        {
            return;
        }
    }

    public void UpdateScoreAndComboUI(int score, int comboMultiplier)
    {
        if (m_ScoreText != null && GameManager.Instance.m_ComboMultiplier >= 1)
        {
            m_ScoreText.text = "Score: " + score + " x" + comboMultiplier;
        }
        else
        {
            return;
        }
    }

    public void UpdateFuelBar(float fuel)
    {
        if (m_FuelBar != null)
        {
            m_FuelBar.value = fuel;
        }
        else
        {
            return;
        }
    }

    public void PauseMenu()
    {
        if (m_PauseMenu == null)
        {
            m_PauseMenu = Resources.Load<GameObject>("Pause");
        }
        if (Time.timeScale == 0)
        {
            ShowCursor(false);
            var pauseObject = m_Canvas.transform.Find("Pause(Clone)");
            if (pauseObject != null)
            {
                DestroyImmediate(pauseObject.gameObject);
            }
            Time.timeScale = 1;
        }
        else if (Time.timeScale == 1)
        {
            SetUpUIControls();
            ShowCursor(true);
            Time.timeScale = 0;
            Instantiate(m_PauseMenu, m_Canvas.transform);
        }
    }

    public void ShowGameOverMenu()
    {
        if (m_GameOverMenu != null)
        {
            ShowCursor(true);
            SetUpUIControls();
            Instantiate(m_GameOverMenu, m_Canvas.transform);
            Time.timeScale = 0;
        }
    }

    public void ShowCursor(bool show)
    {
        if (show)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion

    #region Events
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu")
        {
            m_InputField = GameObject.FindObjectOfType<TMP_InputField>();
            if (m_InputField != null)
            {
                m_InputField.onEndEdit.AddListener(delegate { GameManager.Instance.SetName(m_InputField.text); });
            };
            SetUpUIControls();
        }
        if (scene.name == "Game")
        {
            m_InputField = null;
            m_HealthImage = GameObject.Find("Health").GetComponentsInChildren<Image>();
            m_ScoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
            m_FuelBar = GameObject.Find("FuelBar").GetComponent<Slider>();
            m_PauseMenu = Resources.Load<GameObject>("Pause");
            m_GameOverMenu = Resources.Load<GameObject>("GameOver");
        }
        m_Canvas = GameObject.FindObjectOfType<Canvas>();
    }
    #endregion

    #region Utility/Setup
    public void SetUpUIControls()
    {
        m_InputControls = InputManager.Instance.GetPlayerControls();
        m_InputControls.UI.Menu.performed += TogglePauseMenu;
    }

    public void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            PauseMenu();
        }
    }

    private void CheckForController()
    {
        if (Gamepad.all.Count > 0)
        {
            OnControllerConnected();
        }
    }

    private void OnControllerConnected()
    {
        if (m_FirstSelected == null)
        {
            m_FirstSelected = GameObject.Find("First");
            EventSystem.current.SetSelectedGameObject(m_FirstSelected);
        }
    }
    #endregion
}
