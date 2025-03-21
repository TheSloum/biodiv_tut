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


    #if UNITY_WEBGL && !UNITY_EDITOR
public string folderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
public string folderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif





    private void Awake()
    {
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
        Debug.Log("1");
        if (!Directory.Exists(path))
        {
        Debug.Log("2");
            Debug.LogWarning("The folder does not exist.");
            return true; // Consider nonexistent folders as empty
        }

        Debug.Log(Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0);
        return Directory.GetFiles(path).Length == 0 && Directory.GetDirectories(path).Length == 0;
    }
    public void Resume()
    {
        resumeLoad = true;
        saveDate = null;
        Materials.instance.isLoad = true;
        Materials.instance.tutorial = false;
        Debug.Log("CHIBRE");
        SceneManager.LoadScene("SampleScene");
    }
}
