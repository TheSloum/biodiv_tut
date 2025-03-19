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
    private List<AudioClip> gestionMusics = new List<AudioClip>();
    private List<AudioClip> remainingGestionMusics = new List<AudioClip>();

    void Start()
    {
        gestionMusics.Add(Gestion1);
        gestionMusics.Add(Gestion2);
        gestionMusics.Add(Gestion3);
    }

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
            musiqueList = GetRandomGestionMusics();
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

    private List<AudioClip> GetRandomGestionMusics()
    {
        if (remainingGestionMusics.Count == 0)
        {
            remainingGestionMusics = new List<AudioClip>(gestionMusics);
        }

        int randomIndex = Random.Range(0, remainingGestionMusics.Count);
        AudioClip selectedClip = remainingGestionMusics[randomIndex];

        remainingGestionMusics.RemoveAt(randomIndex);

        return new List<AudioClip> { selectedClip };
    }
}
