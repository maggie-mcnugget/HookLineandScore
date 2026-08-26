using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private void Start()
    {
        // Get the score from the previous game. If there isn't one yet, use 0.
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);

        scoreText.text = "Score: " + lastScore;
    }

    public void PlayGame()
    {
        // Load the actual game scene
        SceneManager.LoadScene("game");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        // Closes the game when running the built application
        Application.Quit();
    }
}
