using UnityEngine;
using UnityEngine.UI;

public class ScreenModeButtonManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button fullScreenButton;
    public Button windowedButton;

    private void Start()
    {
        fullScreenButton.onClick.AddListener(SetFullScreen);
        windowedButton.onClick.AddListener(SetWindowed);

        UpdateButtonStates();
    }

    private void SetFullScreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.fullScreen = true;
        UpdateButtonStates();
        Debug.Log("Mode activé : Plein Écran");
    }

    private void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreen = false;
        UpdateButtonStates();
        Debug.Log("Mode activé : Fenêtré");
    }

    private void UpdateButtonStates()
    {
        bool isFull = Screen.fullScreen;

        fullScreenButton.interactable = !isFull;
        windowedButton.interactable = isFull;
    }
}
