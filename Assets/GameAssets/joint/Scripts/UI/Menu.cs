using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject Parametre;

    public Toggle fontToggle;
    public TMP_FontAsset ClassicFont;
    public TMP_FontAsset disFont;
    public TMP_Text[] textsToChange;

    private int fontSizeOffset = 3; // Décalage de la taille de police

    void Start()
    {
        if (fontToggle != null)
            fontToggle.onValueChanged.AddListener(OnFontToggleChanged);
    }

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

    // Gestion des menus
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

    // Texte dyslexie avec ajustement de taille
    void OnFontToggleChanged(bool isChecked)
    {
        foreach (TMP_Text txt in textsToChange)
        {
            if (txt != null)
            {
                if (isChecked)
                {
                    // Applique la deuxième police et réduit la taille
                    txt.font = disFont;
                    txt.fontSize -= fontSizeOffset;
                }
                else
                {
                    // Applique la police classique et rétablit la taille d'origine
                    txt.font = ClassicFont;
                    txt.fontSize += fontSizeOffset;
                }
            }
        }
    }
}
