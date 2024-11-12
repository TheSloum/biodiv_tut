using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Parametre;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Parametre.activeSelf)
            {
                CloseParametreAndOpenMainMenu();
            }
            else
            {
                ShowOrHideElement();
            }
        }
    }

    public void ShowOrHideElement()
    {
        MainMenu.SetActive(!MainMenu.activeSelf);
        Time.timeScale = MainMenu.activeSelf ? 0 : 1;
    }

    public void SwapElements()
    {
        MainMenu.SetActive(false);
        Parametre.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseParametreAndOpenMainMenu()
    {
        Parametre.SetActive(false);
        MainMenu.SetActive(true);
        Time.timeScale = 0;
    }
}
