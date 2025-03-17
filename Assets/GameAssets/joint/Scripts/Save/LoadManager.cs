using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class LoadManager : MonoBehaviour
{
    public static LoadManager instance;


    public bool resumeLoad;
    public string saveDate = null;



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

    }

    public static void DestroyInstance()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }

    public void Resume()
    {
        resumeLoad = true;
        saveDate = null;
        Materials.instance.isLoad = true;
        Materials.instance.tutorial = false;
        SceneManager.LoadScene("SampleScene");
    }
}
