using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    [SerializeField]
    private AudioClip backgroundMusic;
    [SerializeField]
    private AudioClip buttonClickSound;
    
    private AudioSource _audioSource;

    private static AudioManager _instance;
    public static AudioManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<AudioManager>();
            }
            return _instance;
        }
    }
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        _audioSource = gameObject.AddComponent<AudioSource>();
        PlayBackgroundMusic();
    }
    
    public void PlayButtonClickSound()
    {
        if (buttonClickSound == null)
        {
            Debug.LogWarning("Button click sound clip is not assigned!");
            return;
        }

        _audioSource.PlayOneShot(buttonClickSound);
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("Background music clip is not assigned!");
            return;
        }

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.clip = backgroundMusic;
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f; // 2D sound
        _audioSource.Play();
    }
}
