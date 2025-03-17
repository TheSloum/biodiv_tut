using TMPro;
using UnityEngine;

public class OutlineText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro == null)
        {
            return;
        }

        textMeshPro.outlineWidth = 0.2f;
        textMeshPro.outlineColor = Color.white;
    }
}
