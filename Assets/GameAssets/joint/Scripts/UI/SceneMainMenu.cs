using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMainMenu : MonoBehaviour
{
    public AudioClip sfxClip;

    public GameDataSaver savescript;

public GameObject loadingObject; 
    public float fadeDuration = 1f; 

void Awake(){

        loadingObject = GameObject.Find("loadingScreen");
}
private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingObject.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            yield return null; 
        }

    }


    public void LoadScene(string Exploration_main)
    {
        StartCoroutine(LoadSceneAsync("Exploration_main"));
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1f;
    }


}