using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMainMenu : MonoBehaviour
{
    public AudioClip sfxClip;
    public AudioClip musicClip;
    public void LoadScene(string Exploration_main)
    {
        SoundManager.instance.PlayMusic(musicClip);
        SoundManager.instance.PlaySFX(sfxClip);
        SceneManager.LoadScene(Exploration_main);
    }
}
