using TMPro;
using UnityEngine;
public class cityNameRemplace : MonoBehaviour
{

    public TextMeshProUGUI textMeshPro; // Référence à ton TextMeshProUI

    private void Update()
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = textMeshPro.text.Replace("{Materials.instance.townName}", Materials.instance.townName);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI n'est pas assigné !");
        }
    }
}
