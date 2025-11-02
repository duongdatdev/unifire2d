using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("Score Data")]
    public int score = 0;
    private int highScore = 0;

    private void Awake()
    {
        // Ensure there is only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved high score from local storage
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        // Listen for scene load to update score UI
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Unsubscribe when destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find UI elements in the current scene
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        UpdateScoreUI();
    }

    // Add points to the current score
    public void AddScore(int amount)
    {
        score += amount;

        // Check if a new high score is reached
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        UpdateScoreUI();
    }

    // Update the score and high score UI
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;

        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }

    // Called when the game ends
    public void GameOver()
    {
        // Load the Game Over scene
        SceneManager.LoadScene("GameOverScene");
    }

    // Reset the current score to 0
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    // Clear saved high score (for debugging)
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HighScore");
        highScore = 0;
        PlayerPrefs.Save();
        UpdateScoreUI();
    }
}
