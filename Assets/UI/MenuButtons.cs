using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void PlayGame()
    {
        LoadScene("Game");
        ResetGameState();
    }

    public void Highscore()
    {
        LoadScene("Highscore");
    }

    public void MainMenu()
    {
        LoadScene("Menu");
    }

    public void Credits()
    {
        LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private void ResetGameState()
    {
        GameManager.Instance.m_Score = 0;
        GameManager.Instance.m_Wave = 0;
    }
}
