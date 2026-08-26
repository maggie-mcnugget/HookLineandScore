using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer")]
    public float timeRemaining = 60f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private bool timerRunning = true;

    void Start()
    {
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (!timerRunning)
            return;

        // Time.deltaTime makes the timer count down in real time instead of decreasing by a fixed amount every frame
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            timerRunning = false;

            UpdateTimerDisplay();

            EndGame();
            return;
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        // CeilToInt rounds up so the player sees 60, 59, 58... instead of the decimal value
        int seconds = Mathf.CeilToInt(timeRemaining);

        timerText.text = seconds.ToString();
    }

    private void EndGame()
    {
        Debug.Log("Time's up!");

        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();

        if (scoreManager != null)
        {
            // Save the score before leaving the game scene
            scoreManager.SaveScore();
        }

        // Return to the main menu when the timer reaches zero
        SceneManager.LoadScene("MainMenu");
    }
}