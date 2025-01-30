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
        StartCoroutine(SaveAndLoadScene(Exploration_main));
    }

    private IEnumerator SaveAndLoadScene(string sceneName)
    {

            savescript.SaveData();


        while (!savescript.SaveAndLoadScene)
        {
            yield return null;
        }
        SoundManager.instance.PlayMusic(musicClip);
        SoundManager.instance.PlaySFX(sfxClip);
        SceneManager.LoadScene(sceneName);
    }
}
