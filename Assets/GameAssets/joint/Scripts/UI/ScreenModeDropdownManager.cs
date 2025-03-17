using UnityEngine;
using UnityEngine.UI;

public class ScreenModeDropdownManager : MonoBehaviour
{
    [Header("Dropdown pour le mode d'�cran")]
    public Dropdown screenModeDropdown; 

    private void Start()
    {

        if (screenModeDropdown != null)
        {
            screenModeDropdown.ClearOptions();
            screenModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Plein �cran", "Fen�tr�" });

            screenModeDropdown.value = Screen.fullScreen ? 0 : 1;


            screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);
        }
    }

    private void OnScreenModeChanged(int index)
    {
        if (index == 0) // Plein �cran
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; 
            Screen.fullScreen = true;
            Debug.Log("Mode activ� : Plein �cran");
        }
        else if (index == 1) // Fen�tr�
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; 
            Screen.fullScreen = false;
            Debug.Log("Mode activ� : Fen�tr�");
        }
    }
}

