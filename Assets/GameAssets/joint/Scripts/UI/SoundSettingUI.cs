using UnityEngine;
using UnityEngine.UI;

public class SoundSettingUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider globalSlider;

    private void Start()
    {
        if (SoundManager.instance != null)
        {
            musicSlider.value = SoundManager.instance.musicSource.volume;
            sfxSlider.value = SoundManager.instance.sfxSource.volume;
            globalSlider.value = 1f; // Global commence à 100%
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        globalSlider.onValueChanged.AddListener(SetGlobalVolume);
    }

    private void SetMusicVolume(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetMusicVolume(value);
        }
    }

    private void SetSFXVolume(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetSFXVolume(value);
        }
    }

    private void SetGlobalVolume(float value)
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.SetGlobalVolume(value);
        }
    }
}
