using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score = 0;

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int points)
    {
        score += points;

        UpdateScoreText();
    }

    public int GetScore()
    {
        return score;
    }

    public void SaveScore()
    {
        //lets us keep the score after changing scenes
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}