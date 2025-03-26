using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
public class SceneMainMenu1 : MonoBehaviour
{

    public GameDataSaver savescript;

    public GameObject loadingObject;
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        GameObject loadingObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "loadingScreen");
        loadingObject.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            yield return null;
        }


    }


    private void Awake()
    {
        GameObject loadingObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "loadingScreen");
        savescript = Materials.instance.GetComponent<GameDataSaver>();
    }
    public void ResetGame()
    {
        if (Materials.instance != null)
        {
            Materials.instance.ResetState();
        }

        if (J_TimeManager.Instance != null)
        {
            J_TimeManager.Instance.ResetState();
        }

        if (E_GameManager.instance != null)
        {
            E_GameManager.instance.ResetState();
        }

        Time.timeScale = 1f;

        PlayerPrefs.DeleteAll();

        StartCoroutine(LoadSceneAsync("Menue"));
    }


}
