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
