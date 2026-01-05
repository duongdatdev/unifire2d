using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Score UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    [Header("Life UI")]
    public Transform heartsContainer;
    
    [Header("Player Stats")]
    public int maxLives = 3;
    private int _currentLives;

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

        _currentLives = maxLives;
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
        _currentLives = maxLives;
        UpdateHeartsUI();
        UpdateScoreUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find UI elements in the current scene
        scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();

        StartCoroutine(FindUIElements(scene.name));
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
    
    public void LoseLife()
    {
        _currentLives--;
        UpdateHeartsUI();

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayLoseLifeSound();
        }
        
        if (_currentLives <= 0)
        {
            AudioManager.instance.PlayLoseGameSound();
            GameOver();
        }
    }
    
    public void UpdateHeartsUI()
    {
        if (heartsContainer == null)
        {
            heartsContainer = GameObject.Find("Hearts")?.transform;
            if (heartsContainer == null)
            {
                return;
            }
            Debug.Log("Hearts container not assigned, trying to find it in the scene.");
        }
        
        for (int i = 0; i < heartsContainer.childCount; i++)
        {
            heartsContainer.GetChild(i).gameObject.SetActive(i < _currentLives);
        }
    }
    
    public void ResetLives()
    {
        _currentLives = maxLives;
        UpdateHeartsUI();
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
    
    private IEnumerator FindUIElements(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.05f); // đợi 1 frame để UI được sinh ra

        scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();
        highScoreText = GameObject.Find("HighScoreText")?.GetComponent<TextMeshProUGUI>();
        heartsContainer = GameObject.Find("Hearts")?.transform;

        if (sceneName == "GameplayScene")
        {
            ResetLives();
            ResetScore();
        }

        UpdateScoreUI();
        UpdateHeartsUI();
    }
}
