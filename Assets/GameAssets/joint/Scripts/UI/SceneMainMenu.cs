using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMainMenu : MonoBehaviour
{
    public AudioClip sfxClip;
    public AudioClip musicClip;

    public GameDataSaver savescript;



    public void LoadScene(string Exploration_main)
    {
        Debug.Log("testeridos");
        SceneManager.LoadScene("Menue");

        SoundManager.instance.PlayMusic(musicClip);
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1f;
    }

}
