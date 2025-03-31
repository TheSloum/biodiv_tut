using UnityEngine;

public class E_openMenu : MonoBehaviour
{
    public GameObject menuToShow;

    private void Start()
    {
        if (menuToShow == null)
        {
            Debug.LogError("❌ E_openMenu: Aucun menu assigné ! Vérifie dans l'inspecteur.");
        }
    }

    private void Update()
    {
        // Vérifier si on est sur WebGL
        bool isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

        // Définir la touche en fonction de la plateforme
        KeyCode toggleKey = isWebGL ? KeyCode.P : KeyCode.Escape;

        // Si la touche est pressée, toggle le menu
        if (Input.GetKeyDown(toggleKey))
        {
            if (Materials.instance.tutorial == false)
            {
                ToggleMenu();
            }
        }
    }

    public void ToggleMenu()
    {
        if (menuToShow != null)
        {
            bool isActive = menuToShow.activeSelf;
            menuToShow.SetActive(!isActive);
            Time.timeScale = menuToShow.activeSelf ? 0f : 1f;
        }
        else
        {
            Debug.LogError("❌ Impossible de toggle le menu: menuToShow est NULL !");
        }
    }
}
