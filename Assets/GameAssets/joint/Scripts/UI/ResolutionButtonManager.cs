using UnityEngine;
using UnityEngine.UI;

public class ResolutionButtonManager : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button res1600x900Button;
    public Button res1920x1080Button;
    public Button res2560x1440Button;

    private void Start()
    {
        res1600x900Button.onClick.AddListener(() => SetResolution(1600, 900));
        res1920x1080Button.onClick.AddListener(() => SetResolution(1920, 1080));
        res2560x1440Button.onClick.AddListener(() => SetResolution(2560, 1440));

        UpdateButtonStates();
    }

    private void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, Screen.fullScreenMode);
        Debug.Log($"Résolution appliquée : {width}x{height}");
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        int currentW = Screen.width;
        int currentH = Screen.height;

        res1600x900Button.interactable = !(currentW == 1600 && currentH == 900);
        res1920x1080Button.interactable = !(currentW == 1920 && currentH == 1080);
        res2560x1440Button.interactable = !(currentW == 2560 && currentH == 1440);
    }
}
