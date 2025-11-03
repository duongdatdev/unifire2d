using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject instructionsPanel;

    public void PlayGame()
    {
        AudioManager.instance.PlayButtonClickSound();
        SceneManager.LoadScene("GameplayScene");
    }

    public void ShowInstructions()
    {
        AudioManager.instance.PlayButtonClickSound();
        instructionsPanel.SetActive(true);
    }

    public void HideInstructions()
    {
        AudioManager.instance.PlayButtonClickSound();
        instructionsPanel.SetActive(false);
    }

    public void QuitGame()
    {
        AudioManager.instance.PlayButtonClickSound();
        Debug.Log("Quit Game");
        Application.Quit();
    }
}