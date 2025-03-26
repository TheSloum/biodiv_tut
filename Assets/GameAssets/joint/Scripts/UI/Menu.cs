using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class Menu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject MainMenu;
    public GameObject Parametre;
    public GameObject Loadeur;
    public GameObject RetourLoadeur;
    public Toggle fontToggle;
    public TMP_FontAsset ClassicFont;
    public AudioClip sfxClip;
    public TMP_FontAsset disFont;
    public TMP_Text[] textsToChange;
    public TMP_Text hoverText;
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

    void Awake(){
        fontToggle.isOn = Materials.instance.dys;
    }

    void Update()
    {
        fontToggle.isOn = Materials.instance.dys;
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
        if (!Materials.instance.tutorial)
        {
            KeyCode menuKey = (Application.platform == RuntimePlatform.WebGLPlayer) ? KeyCode.P : KeyCode.Escape;

            if (Input.GetKeyDown(menuKey))
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
    public void OpenLoadeurUniq()
    {
        RetourLoadeur.SetActive(false);
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
        Materials.instance.dys = isChecked;
        TMP_FontAsset targetFont = isChecked ? disFont : ClassicFont;
        TMP_FontAsset oldFont = isChecked ? ClassicFont : disFont;

        TMP_Text[] allTexts = FindObjectsOfType<TMP_Text>(true); // Récupère tous les TMP_Text, y compris ceux désactivés

        foreach (TMP_Text txt in allTexts)
        {
            if (txt != null && txt.font == oldFont) // Vérifie si le texte utilise l'ancienne police
            {
                txt.font = targetFont;

                // Réduction encore plus forte de la taille
                txt.fontSize += isChecked ? -fontSizeOffset * 2 : fontSizeOffset * 2;

                // Réduction de l'interligne
                txt.lineSpacing = isChecked ? -3f : 0f;
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
