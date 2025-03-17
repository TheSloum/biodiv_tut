using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMainMenu : MonoBehaviour
{
    public AudioClip sfxClip;

    public GameDataSaver savescript;



    public void LoadScene(string Exploration_main)
    {
        SceneManager.LoadScene("Exploration_main");
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1f;
    }

}
