using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneMainMenu : MonoBehaviour
{
    public void LoadScene(string SampleScene)
    {
        SceneManager.LoadScene(SampleScene);
    }
}
