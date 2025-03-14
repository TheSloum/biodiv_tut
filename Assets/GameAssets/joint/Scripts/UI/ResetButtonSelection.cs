using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; 

public class ResetButtonSelection : MonoBehaviour
{
    public Button defaultButton; 

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);


        if (defaultButton != null)
        {
            ColorBlock colors = defaultButton.colors;
            colors.normalColor = Color.white; 
            defaultButton.colors = colors;
        }
    }
}
