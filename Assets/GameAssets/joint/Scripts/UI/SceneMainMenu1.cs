using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMainMenu1 : MonoBehaviour
{
    public AudioClip sfxClip;

    public GameDataSaver savescript;



    public void LoadScene(string Menue)
    {
        SceneManager.LoadScene("Menue");
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1f;
    }

}
