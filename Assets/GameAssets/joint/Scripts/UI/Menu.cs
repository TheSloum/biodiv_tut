using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Parametre;

    public GameObject Loadeur;

    public Toggle fontToggle;
    public TMP_FontAsset ClassicFont;
    public TMP_FontAsset disFont;
    public TMP_Text[] textsToChange;

    private int fontSizeOffset = 2;

    void Start()
    {
        if (fontToggle != null)
            fontToggle.onValueChanged.AddListener(OnFontToggleChanged);
    }

    void Update()
    {
        if (Parametre.activeSelf || MainMenu.activeSelf )
        {
            Materials.instance.canMove = false;
        }else if(!Materials.instance.tutorial){
            Materials.instance.canMove = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Parametre.activeSelf)
            {
                CloseParametreAndOpenMainMenu();
            }
            else if (Loadeur.activeSelf)
            {
                CloseLoadeur();
            }

            else
            {
                ShowOrHideMainMenue();
            }
        }
    }

    // Gestion des menus
    public void ShowOrHideMainMenue()
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
    public void CloseLoadeur()
    {
        ShowOrHideMainMenue();
        Loadeur.SetActive(false);
    }
    public void OpenLoadeur()
    {
        ShowOrHideMainMenue();
        Loadeur.SetActive(true);
    }

    public void CloseParametreAndOpenMainMenu()
    {
        Parametre.SetActive(false);
        MainMenu.SetActive(true);
        Time.timeScale = 0;
    }
    void OnFontToggleChanged(bool isChecked)
    {
        foreach (TMP_Text txt in textsToChange)
        {
            if (txt != null)
            {
                if (isChecked)
                {

                    txt.font = disFont;
                    txt.fontSize -= fontSizeOffset;
                }
                else
                {

                    txt.font = ClassicFont;
                    txt.fontSize += fontSizeOffset;
                }
            }
        }
    }
}
