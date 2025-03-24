using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneMainMenu1 : MonoBehaviour
{

    public GameDataSaver savescript;

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

        SceneManager.LoadScene("Menue");
    }


}
