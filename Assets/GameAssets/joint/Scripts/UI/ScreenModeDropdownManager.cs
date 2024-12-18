using UnityEngine;
using UnityEngine.UI;

public class ScreenModeDropdownManager : MonoBehaviour
{
    [Header("Dropdown pour le mode d'écran")]
    public Dropdown screenModeDropdown; 

    private void Start()
    {

        if (screenModeDropdown != null)
        {
            screenModeDropdown.ClearOptions();
            screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Plein Écran", "Fenêtré" });

            screenModeDropdown.value = Screen.fullScreen ? 0 : 1;


            screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        }
    }

    private void OnScreenModeChanged(int index)
    {
        if (index == 0) // Plein Écran
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; 
            Screen.fullScreen = true;
            Debug.Log("Mode activé : Plein Écran");
        }
        else if (index == 1) // Fenêtré
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; 
            Screen.fullScreen = false;
            Debug.Log("Mode activé : Fenêtré");
        }
    }
}

