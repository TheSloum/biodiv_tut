using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SondScene : MonoBehaviour
{
    public static SondScene instance;

    public AudioClip Gestion1;
    public AudioClip Gestion2;
    public AudioClip Gestion3;
    public AudioClip Exploration;
    public AudioClip Menue;
    private string currentScene = "";

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName != currentScene)
        {
            currentScene = sceneName;
            PlayMusicForScene(sceneName);
        }
    }

    private void PlayMusicForScene(string sceneName)
    {
        if (SoundManager.instance == null)
        {
            Debug.LogError("SoundManager.instance est null !");
            return;
        }

        List<AudioClip> musiqueList = new List<AudioClip>();

        if (sceneName == "SampleScene")
        {
            musiqueList.Add(Gestion1);
            musiqueList.Add(Gestion2);
            musiqueList.Add(Gestion3);
        }
        else if (sceneName == "Exploration_main")
        {
            musiqueList.Add(Exploration);
        }
        else
        {
            musiqueList.Add(Menue);
        }

        if (musiqueList.TrueForAll(clip => clip != null))
        {
            SoundManager.instance.PlayMusicSequence(musiqueList, 0f);
        }
        else
        {
            Debug.LogWarning("Un ou plusieurs clips audio sont null !");
        }
    }
}
