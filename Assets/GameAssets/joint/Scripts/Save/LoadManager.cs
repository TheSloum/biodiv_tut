using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;

    public GameObject resumeButton;
    public GameObject resumeButtonDis;

    public bool resumeLoad;
    public string saveDate = null;
    public GameObject loadingObject; 


#if UNITY_WEBGL && !UNITY_EDITOR
public string folderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
    public string folderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif




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


    private void Awake()
    {
    loadingObject = GameObject.Find("loadingScreen");
    
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Materials.instance.menuFirst = false;

    }
    void Start()
    {
        if (IsFolderEmpty(folderPath))
        {
            resumeButton.SetActive(false);
            resumeButtonDis.SetActive(true);
        }
    }

    public static void DestroyInstance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }


    bool IsFolderEmpty(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogWarning("The folder does not exist.");
            return true; // Consider nonexistent folders as empty
        }

        return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
    }
    public void Resume()
    {

        Debug.Log("AEGPKJZORJGTPOJ");
        resumeLoad = true;
        saveDate = null;
        Materials.instance.isLoad = true;
        Materials.instance.tutorial = false;
        StartCoroutine(LoadSceneAsync("SampleScene"));
    }
}
