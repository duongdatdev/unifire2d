using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip buttonClickSound;

    [Header("Gameplay Sounds")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip loseLifeSound;
    [SerializeField] private AudioClip loseGameSound;
    
    [Header("Background Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Add 2 separate AudioSources: one for music, one for SFX
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.spatialBlend = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.spatialBlend = 0f;

        // Listen for scene changes
        SceneManager.activeSceneChanged += OnSceneChanged;

        // Play the initial music
        ChangeMusicByScene(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        ChangeMusicByScene(newScene.name);
    }

    // Change background music depending on scene name
    private void ChangeMusicByScene(string sceneName)
    {
        AudioClip newClip = null;
        float newVolume = 1f;

        switch (sceneName)
        {
            case "MainMenuScene":
                newClip = mainMenuMusic;
                newVolume = 2f;
                break;

            case "GameplayScene":
                newClip = gameplayMusic;
                newVolume = 0.5f;
                break;

            case "GameOverScene":
                newClip = gameOverMusic;
                newVolume = 0.5f;
                break;

            default:
                // fallback music if scene is unrecognized
                newClip = mainMenuMusic;
                break;
        }

        if (newClip != null && musicSource.clip != newClip)
        {
            PlayMusic(newClip, newVolume);
        }
    }

    private void PlayMusic(AudioClip clip, float targetVolume)
    {
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.volume = targetVolume;
        musicSource.Play();
    }

    public void PlayButtonClickSound()
    {
        if (buttonClickSound == null)
        {
            Debug.LogWarning("Button click sound not assigned!");
            return;
        }

        sfxSource.PlayOneShot(buttonClickSound);
    }
    
    public void PlayShootSound()
    {
        if (shootSound == null)
        {
            Debug.LogWarning("Shoot sound not assigned!");
            return;
        }

        sfxSource.PlayOneShot(shootSound, 0.6f);
    }
    
    public void PlayExplosionSound()
    {
        if (explosionSound == null)
        {
            Debug.LogWarning("Explosion sound not assigned!");
            return;
        }

        sfxSource.PlayOneShot(explosionSound);
    }
    
    public void PlayLoseLifeSound()
    {
        if (loseLifeSound == null)
        {
            Debug.LogWarning("Lose life sound not assigned!");
            return;
        }

        sfxSource.PlayOneShot(loseLifeSound);
    }
    
    public void PlayLoseGameSound()
    {
        if (loseGameSound == null)
        {
            Debug.LogWarning("Lose game sound not assigned!");
            return;
        }

        sfxSource.PlayOneShot(loseGameSound);
    }

    // Optional: fade transition between songs
    public void FadeToMusic(AudioClip newClip, float fadeTime = 1f)
    {
        StartCoroutine(FadeMusicRoutine(newClip, fadeTime));
    }

    private System.Collections.IEnumerator FadeMusicRoutine(AudioClip newClip, float fadeTime)
    {
        float startVolume = musicSource.volume;

        // Fade out
        while (musicSource.volume > 0)
        {
            musicSource.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        while (musicSource.volume < startVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }
    }
}
