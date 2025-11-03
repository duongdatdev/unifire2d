using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;

    private void Start()
    {
        // Show the final score from the GameManager
        if (finalScoreText != null)
            finalScoreText.text = "Score: " + GameManager.Instance.score.ToString();

        // Show the current high score
        if (highScoreText != null)
            highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore", 0).ToString();
    }

    public void RetryGame()
    {
        GameManager.Instance.ResetLives();
        GameManager.Instance.ResetScore();
        AudioManager.instance.PlayButtonClickSound();
        SceneManager.LoadScene("GamePlayScene");
    }

    public void BackToMenu()
    {
        AudioManager.instance.PlayButtonClickSound();
        SceneManager.LoadScene("MainMenuScene");
    }
}
