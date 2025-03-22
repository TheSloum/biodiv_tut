using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Nécessaire pour gérer les événements de la souris

public class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject MainMenu;
    public GameObject Parametre;
    public GameObject Loadeur;
    public Toggle fontToggle;
    public TMP_FontAsset ClassicFont;
    public AudioClip sfxClip;
    public TMP_FontAsset disFont;
    public TMP_Text[] textsToChange;
    public TMP_Text hoverText; // Texte qui aura un contour et changera de couleur au survol
    private float IsInMenue = 0;
    private int fontSizeOffset = 2;

    void Start()
    {
        IsInMenue = 0;
        if (PlayerPrefs.HasKey("FontToggleState"))
        {
            bool isChecked = PlayerPrefs.GetInt("FontToggleState") == 1;
            fontToggle.isOn = isChecked;
            ApplyFontChange(isChecked);
        }

        if (fontToggle != null)
        {
            fontToggle.onValueChanged.AddListener(OnFontToggleChanged);
        }
    }

    void Update()
    {
        if (Parametre.activeSelf || MainMenu.activeSelf || Loadeur.activeSelf && IsInMenue == 0)
        {
            Materials.instance.canMove = false;
            IsInMenue = 1;
        }
        else if (!Materials.instance.tutorial && !Parametre.activeSelf && !MainMenu.activeSelf && !Loadeur.activeSelf && IsInMenue == 1)
        {
            Materials.instance.canMove = true;
            IsInMenue = 0;
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

    public void ShowOrHideMainMenu()
    {
        MainMenu.SetActive(!MainMenu.activeSelf);
        Time.timeScale = MainMenu.activeSelf ? 0 : 1;
        SoundManager.instance.PlaySFX(sfxClip);
    }

    public void SwapElements()
    {
        MainMenu.SetActive(false);
        Parametre.SetActive(true);
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 0;
    }

    public void CloseLoadeur()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        ShowOrHideMainMenu();
        Loadeur.SetActive(false);
    }

    public void OpenLoadeur()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        ShowOrHideMainMenu();
        Loadeur.SetActive(true);
    }

    public void CloseParametreAndOpenMainMenu()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Parametre.SetActive(false);
        MainMenu.SetActive(true);
        Time.timeScale = 0;
    }

    void OnFontToggleChanged(bool isChecked)
    {
        SoundManager.instance.PlaySFX(sfxClip);
        PlayerPrefs.SetInt("FontToggleState", isChecked ? 1 : 0);
        PlayerPrefs.Save();
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

    // Gestion du survol pour ajouter un contour et changer la couleur du texte
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverText != null)
        {
            // Appliquer le contour sur le texte au survol
            hoverText.outlineWidth = 0.2f; // Ajuste l'épaisseur du contour
            hoverText.outlineColor = Color.white; // Définit la couleur du contour à blanc

            // Changer la couleur du texte lors du survol
            hoverText.color = Color.red; // Changer la couleur du texte en rouge (tu peux mettre la couleur que tu veux)
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoverText != null)
        {
            // Retirer le contour lorsque la souris quitte
            hoverText.outlineWidth = 0f; // Retirer le contour

            // Réinitialiser la couleur du texte lorsque la souris quitte
            hoverText.color = Color.white; // Remettre la couleur initiale du texte (blanc par exemple)
        }
    }
}
