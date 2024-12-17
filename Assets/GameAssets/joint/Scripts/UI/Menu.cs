using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject MainMenu; // Référence au menu principal
    public GameObject Parametre; // Référence aux paramètres

    [Header("Font Changer Settings")]
    public Toggle fontToggle; // La case à cocher (Toggle) pour changer de police
    public TMP_FontAsset oldFont; // Police par défaut
    public TMP_FontAsset newFont; // Nouvelle police
    public TMP_Text[] textsToChange; // Liste des textes à changer

    void Start()
    {
        // Attache l'écouteur pour le Toggle
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

    // Méthode appelée quand la case est cochée/décochée
    void OnFontToggleChanged(bool isChecked)
    {
        foreach (TMP_Text txt in textsToChange)
        {
            if (txt != null)
            {
                txt.font = isChecked ? newFont : oldFont;
            }
        }
    }
}
