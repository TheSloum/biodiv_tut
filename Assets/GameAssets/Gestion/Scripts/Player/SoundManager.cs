using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource musicSource; 
    public AudioSource sfxSource;  
    

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
                musicSource.clip = clip;
                musicSource.Play();
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
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = Mathf.Clamp01(volume);
    }
    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = Mathf.Clamp01(volume);
    }
}
