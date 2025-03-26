using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ResolutionDropdownManager : MonoBehaviour
{
    [Header("Boutons de résolution")]
    public Button button1600x900;
    public Button button1920x1080;
    public Button button2560x1440;

    [Header("Textes des boutons")]
    public TextMeshProUGUI text1600x900;
    public TextMeshProUGUI text1920x1080;
    public TextMeshProUGUI text2560x1440;

    [Header("Sprites pour l'état des boutons")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    [Header("Couleurs du texte")]
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = Color.gray;
    public Color hoverTextColor = Color.yellow;

    private Button currentActiveButton;
    private TextMeshProUGUI currentActiveText;

    private void Start()
    {
        // Ajouter des listeners pour les boutons
        button1600x900.onClick.AddListener(() => SetResolution(1600, 900, button1600x900, text1600x900));
        button1920x1080.onClick.AddListener(() => SetResolution(1920, 1080, button1920x1080, text1920x1080));
        button2560x1440.onClick.AddListener(() => SetResolution(2560, 1440, button2560x1440, text2560x1440));

        // Ajouter effets de survol
        AddHoverEffects(button1600x900, text1600x900);
        AddHoverEffects(button1920x1080, text1920x1080);
        AddHoverEffects(button2560x1440, text2560x1440);

        // Vérifier la résolution actuelle pour initialiser l'affichage
        if (Screen.currentResolution.width == 1600 && Screen.currentResolution.height == 900)
            SetResolution(1600, 900, button1600x900, text1600x900);
        else if (Screen.currentResolution.width == 1920 && Screen.currentResolution.height == 1080)
            SetResolution(1920, 1080, button1920x1080, text1920x1080);
        else
            SetResolution(2560, 1440, button2560x1440, text2560x1440);
    }

    void Update(){
        if (
        Materials.instance.resX == 1600)
            SetResolution(1600, 900, button1600x900, text1600x900);
        else if (Materials.instance.resX == 1920)
            SetResolution(1920, 1080, button1920x1080, text1920x1080);
        else
            SetResolution(2560, 1440, button2560x1440, text2560x1440);
    }

    private void SetResolution(int width, int height, Button selectedButton, TextMeshProUGUI selectedText)
    {
        Materials.instance.resX = width;
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        Debug.Log($"Résolution appliquée : {width}x{height}");

        // Remettre l'ancien bouton en inactif
        if (currentActiveButton != null && currentActiveText != null)
        {
            currentActiveButton.GetComponent<Image>().sprite = inactiveSprite;
            currentActiveText.color = inactiveTextColor;
        }

        // Activer le nouveau bouton sélectionné
        selectedButton.GetComponent<Image>().sprite = activeSprite;
        selectedText.color = activeTextColor;

        // Mettre à jour la référence du bouton actif
        currentActiveButton = selectedButton;
        currentActiveText = selectedText;
    }

    private void AddHoverEffects(Button button, TextMeshProUGUI buttonText)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Survol (PointerEnter)
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnHoverEnter(button, buttonText); });
        trigger.triggers.Add(entryEnter);

        // Sortie de survol (PointerExit)
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnHoverExit(button, buttonText); });
        trigger.triggers.Add(entryExit);
    }

    private void OnHoverEnter(Button button, TextMeshProUGUI buttonText)
    {
        if (button == currentActiveButton) return; // Ne pas changer si déjà actif

        button.GetComponent<Image>().sprite = activeSprite;
        buttonText.color = hoverTextColor;
    }

    private void OnHoverExit(Button button, TextMeshProUGUI buttonText)
    {
        if (button == currentActiveButton) return; // Ne pas changer si déjà actif

        button.GetComponent<Image>().sprite = inactiveSprite;
        buttonText.color = inactiveTextColor;
    }
}
