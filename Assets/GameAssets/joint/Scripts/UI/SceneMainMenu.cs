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
         GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            Destroy(gameManager);
        }

        Materials.instance = null; 

        SceneManager.LoadScene(0);
        SoundManager.instance.PlayMusic(musicClip);
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1f;
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
        Time.timeScale = 1f;
    }
}
