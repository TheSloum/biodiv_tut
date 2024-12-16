using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    private float globalVolume = 1f;
    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        globalVolume = PlayerPrefs.GetFloat("GlobalVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        ApplyVolumes();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetFloat("GlobalVolume", globalVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioClip est null dans PlaySFX()");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            if (musicSource.clip != clip)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }

                fadeCoroutine = StartCoroutine(FadeOutAndChangeMusic(clip));
            }
        }
        else
        {
            Debug.LogWarning("AudioClip est null dans PlayMusic()");
        }
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void SetGlobalVolume(float volume)
    {
        globalVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * globalVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume * globalVolume;
        }
    }

    private IEnumerator FadeOutAndChangeMusic(AudioClip newClip)
    {
        // Fade out current music
        float startVolume = musicSource.volume;
        while (musicSource.volume > 0.01f)
        {
            musicSource.volume -= startVolume * Time.deltaTime / 0.5f; // 1f is the fade duration (adjustable)
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();


        while (musicSource.volume < musicVolume * globalVolume)
        {
            musicSource.volume += startVolume * Time.deltaTime / 1f; // 1f is the fade duration (adjustable)
            yield return null;
        }

        musicSource.volume = musicVolume * globalVolume;
    }
}