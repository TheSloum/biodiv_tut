using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; // Nécessaire pour gérer les événements UI

public class ScreenModeDropdownManager : MonoBehaviour
{
    [Header("Boutons pour changer le mode d'écran")]
    public Button fullScreenButton;
    public Button windowedButton;

    [Header("Textes des boutons")]
    public TextMeshProUGUI fullScreenText;
    public TextMeshProUGUI windowedText;

    [Header("Sprites pour l'état des boutons")]
    public Sprite fullScreenActiveSprite;
    public Sprite fullScreenInactiveSprite;
    public Sprite windowedActiveSprite;
    public Sprite windowedInactiveSprite;

    [Header("Couleurs du texte")]
    public Color activeTextColor = Color.white;
    public Color inactiveTextColor = Color.gray;
    public Color hoverTextColor = Color.white; // Couleur au survol

    public bool isFullScreen;

    private void Start()
    {
        // Ajouter des listeners aux boutons
        fullScreenButton.onClick.AddListener(SetFullScreen);
        windowedButton.onClick.AddListener(SetWindowed);

        // Ajouter les événements de survol
        AddHoverEffects(fullScreenButton, true);
        AddHoverEffects(windowedButton, false);

        // Initialiser l'état des boutons et textes
        isFullScreen = Screen.fullScreen;
        UpdateButtonUI(isFullScreen);
    }

    private void SetFullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
        isFullScreen = true;
        UpdateButtonUI(true);
        Materials.instance.fulls = true;
        Debug.Log("Mode activé : Plein écran");
    }

    void Update(){
        UpdateButtonUI(Materials.instance.fulls);
    }

    private void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
        Materials.instance.fulls = false;
        isFullScreen = false;
        UpdateButtonUI(false);
        Debug.Log("Mode activé : Fenêtré");
    }

    private void UpdateButtonUI(bool isFullScreen)
    {
        Image fullScreenImage = fullScreenButton.GetComponent<Image>();
        Image windowedImage = windowedButton.GetComponent<Image>();

        // Changer les sprites
        fullScreenImage.sprite = isFullScreen ? fullScreenActiveSprite : fullScreenInactiveSprite;
        windowedImage.sprite = isFullScreen ? windowedInactiveSprite : windowedActiveSprite;

        // Changer la couleur du texte
        fullScreenText.color = isFullScreen ? activeTextColor : inactiveTextColor;
        windowedText.color = isFullScreen ? inactiveTextColor : activeTextColor;
    }

    private void AddHoverEffects(Button button, bool isFullScreenButton)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Survol (PointerEnter)
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;
        entryEnter.callback.AddListener((data) => { OnHoverEnter(isFullScreenButton); });
        trigger.triggers.Add(entryEnter);

        // Sortie de survol (PointerExit)
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { OnHoverExit(); });
        trigger.triggers.Add(entryExit);
    }

    private void OnHoverEnter(bool isFullScreenButton)
    {
        if ((isFullScreenButton && isFullScreen) || (!isFullScreenButton && !isFullScreen))
            return; // Ne rien faire si le bouton est déjà actif

        // Appliquer l'effet hover
        Image buttonImage = isFullScreenButton ? fullScreenButton.GetComponent<Image>() : windowedButton.GetComponent<Image>();
        TextMeshProUGUI buttonText = isFullScreenButton ? fullScreenText : windowedText;

        buttonImage.sprite = isFullScreenButton ? fullScreenActiveSprite : windowedActiveSprite;
        buttonText.color = hoverTextColor;
    }

    private void OnHoverExit()
    {
        UpdateButtonUI(isFullScreen);
    }
}
