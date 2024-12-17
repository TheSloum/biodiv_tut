using UnityEngine;
using UnityEngine.SceneManagement;

public class E_AudioManager : MonoBehaviour
{
    public static E_AudioManager instance;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip popSound;

    [Header("Settings")]
    public float backgroundMusicVolume = 0.2f;
    public float popSoundVolume = 1.0f;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    void Awake()
    {
        // Implémentation du Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Configuration des AudioSources
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = backgroundMusicVolume;
            musicSource.pitch = 1f; // Pitch demandé

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = popSoundVolume;

            // Abonnement à l'événement de changement de scène
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBackgroundMusic();
    }

    void OnDestroy()
    {
        // Désabonnement de l'événement pour éviter les erreurs
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Arrêter la musique lors du chargement d'une nouvelle scène
        StopBackgroundMusic();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null)
        {
            musicSource.Play();
            Debug.Log("Musique de fond jouée.");
        }
        else
        {
            Debug.LogWarning("Clip de musique de fond non assigné dans l'E_AudioManager.");
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("Musique de fond arrêtée.");
        }
    }

    public void PlayPopSound()
    {
        if (popSound != null)
        {
            sfxSource.PlayOneShot(popSound);
            Debug.Log("Effet sonore 'pop' joué.");
        }
        else
        {
            Debug.LogWarning("Clip d'effet sonore 'pop' non assigné dans l'E_AudioManager.");
        }
    }
}
