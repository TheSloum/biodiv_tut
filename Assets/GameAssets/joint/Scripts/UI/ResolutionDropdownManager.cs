using UnityEngine;
using UnityEngine.UI;

public class ResolutionDropdownManager : MonoBehaviour
{
    [Header("Dropdown pour les résolutions")]
    public Dropdown resolutionDropdown; 

    private Resolution[] resolutions; 
    private int currentResolutionIndex; 

    private void Start()
    {

        resolutions = new Resolution[]
        {
            new Resolution { width = 800, height = 600 },
            new Resolution { width = 1024, height = 768 },
            new Resolution { width = 1128, height = 634 },
            new Resolution { width = 1280, height = 720 },
            new Resolution { width = 1280, height = 1024 },
            new Resolution { width = 1366, height = 768 },
            new Resolution { width = 1600, height = 900 },
            new Resolution { width = 1680, height = 1050 },
            new Resolution { width = 1760, height = 990 },
            new Resolution { width = 1920, height = 1080 },
            new Resolution { width = 2560, height = 1440 },
            new Resolution { width = 3840, height = 2160 }
        };

        resolutionDropdown.ClearOptions();
        System.Collections.Generic.List<string> options = new System.Collections.Generic.List<string>();

 
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            // Vérifie si c'est la résolution actuelle de l'écran
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // Définir la valeur actuelle dans le Dropdown
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();


        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void SetResolution(int resolutionIndex)
    {
        Resolution selectedResolution = resolutions[resolutionIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreenMode);

        Debug.Log($"Résolution appliquée : {selectedResolution.width}x{selectedResolution.height}, Mode : {Screen.fullScreenMode}");
    }
}
