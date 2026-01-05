using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System;
public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private CanvasGroup canvasGroup;
    public GameObject pauseButton; 

    private UnifireInputs controls;
    private bool isPaused = false;
    
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    private void Awake()
    {
        // Initialize input system
        controls = new UnifireInputs();

        // Subscribe to Pause action
        controls.Gameplay.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        AudioManager.instance?.PlayButtonClickSound();

        pauseMenuUI.SetActive(true);
        pauseButton.SetActive(false); 
        StartCoroutine(FadeCanvas(0f, 1f, 0.3f));

        Time.timeScale = 0f;  // Freeze time
        isPaused = true;
        
        OnGamePaused?.Invoke(); 
    }

    public void ResumeGame()
    {
        AudioManager.instance?.PlayButtonClickSound();

        pauseButton.SetActive(true);
        StartCoroutine(FadeCanvas(1f, 0f, 0.3f));
        StartCoroutine(DisableAfterFade(0.3f));

        Time.timeScale = 1f;  // Resume time
        isPaused = false;
        
        OnGameResumed?.Invoke();
    }

    public void BackToMainMenu()
    {
        AudioManager.instance?.PlayButtonClickSound();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetScore();
        }
        
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        AudioManager.instance?.PlayButtonClickSound();
        Application.Quit();
    }

    private IEnumerator FadeCanvas(float start, float end, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(start, end, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private IEnumerator DisableAfterFade(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (!isPaused)
            pauseMenuUI.SetActive(false);
    }
}
