using UnityEngine;
using UnityEngine.UI;
 
public class SoundSettingUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider globalSlider;
    private bool openit;

    private void Start()
    {
        if (SoundManager.instance != null)
        {
            musicSlider.value = SoundManager.instance.musicVolume;
            sfxSlider.value = SoundManager.instance.sfxVolume;
            globalSlider.value = SoundManager.instance.globalVolume; 
        }

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        globalSlider.onValueChanged.AddListener(SetGlobalVolume);
    }

    void Awake(){
        
        openit = true;
            musicSlider.value = SoundManager.instance.musicVolume;
            sfxSlider.value = SoundManager.instance.sfxVolume;
            globalSlider.value = SoundManager.instance.globalVolume; 
    musicSlider.value = musicSlider.value;
    sfxSlider.value = sfxSlider.value;
    globalSlider.value = globalSlider.value;

    
    }
     void OnEnable()
    {
            musicSlider.value = SoundManager.instance.musicVolume;
            sfxSlider.value = SoundManager.instance.sfxVolume;
            globalSlider.value = SoundManager.instance.globalVolume; 
    musicSlider.value = musicSlider.value;
    sfxSlider.value = sfxSlider.value;
    globalSlider.value = globalSlider.value;
            Debug.Log(musicSlider.value);
            Debug.Log(sfxSlider.value);
            Debug.Log(globalSlider.value);
            
            
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
