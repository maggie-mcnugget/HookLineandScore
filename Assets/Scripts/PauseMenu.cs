using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject gameUI;
    public GameObject pauseMenu;

    private bool isPaused = false;
    private bool isMuted = false;

    public void PauseGame()
    {
        isPaused = true;

        // Hide the normal game UI
        gameUI.SetActive(false);

        pauseMenu.SetActive(true);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;

        pauseMenu.SetActive(false);

        gameUI.SetActive(true);

        Time.timeScale = 1f;
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (isMuted)
        {
            AudioListener.volume = 0f;
        }
        else
        {
            AudioListener.volume = 1f;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}