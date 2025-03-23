using UnityEngine;

public class E_openMenu : MonoBehaviour
{
    public GameObject menuToShow;

    public void ToggleMenu()
    {
        if (menuToShow != null)
        {
            bool isActive = menuToShow.activeSelf;
            menuToShow.SetActive(!isActive);
        }
    }
}
