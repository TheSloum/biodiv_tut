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
        // Charger l'état du Toggle depuis PlayerPrefs
        if (PlayerPrefs.HasKey("FontToggleState"))
        {
            bool isChecked = PlayerPrefs.GetInt("FontToggleState") == 1;
            fontToggle.isOn = isChecked; // Appliquer l'état au Toggle
            ApplyFontChange(isChecked); // Appliquer immédiatement la police
        }

        if (fontToggle != null)
        {
            fontToggle.onValueChanged.AddListener(OnFontToggleChanged);
        }
    }

    void Update()
    {
        if (Parametre.activeSelf || Loadeur.activeSelf || MainMenu.activeSelf)
        {
            Materials.instance.canMove = false;
        }
        else
        {
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
                ShowOrHideMainMenu();
            }
        }
    }

    // Gestion des menus
    public void ShowOrHideMainMenu()
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
        ShowOrHideMainMenu();
        Loadeur.SetActive(false);
    }

    public void OpenLoadeur()
    {
        ShowOrHideMainMenu();
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
        // Sauvegarder l'état du Toggle
        PlayerPrefs.SetInt("FontToggleState", isChecked ? 1 : 0);
        PlayerPrefs.Save(); // S'assurer que les données sont bien enregistrées

        // Appliquer les changements de police
        ApplyFontChange(isChecked);
    }

    void ApplyFontChange(bool isChecked)
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
