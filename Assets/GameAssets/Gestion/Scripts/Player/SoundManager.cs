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

    private int currentMusicToken = 0;

private IEnumerator FadeOutAndChangeMusic(AudioClip newClip)
{
    int thisMusicToken = ++currentMusicToken;
    float startVolume = musicSource.volume;
    float fadeDuration = 0.5f;
    float timer = 0f;

    while (timer < fadeDuration)
    {
        if (thisMusicToken != currentMusicToken) yield break;

        timer += Time.deltaTime;
        musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
        yield return null;
    }
    if (thisMusicToken == currentMusicToken)
    {
        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();
    }
    timer = 0f;
    while (timer < fadeDuration)
    {
        if (thisMusicToken != currentMusicToken) yield break;

        timer += Time.deltaTime;
        musicSource.volume = Mathf.Lerp(0f, musicVolume * globalVolume, timer / fadeDuration);
        yield return null;
    }
    if (thisMusicToken == currentMusicToken)
    {
        musicSource.volume = musicVolume * globalVolume;
    }
}

}