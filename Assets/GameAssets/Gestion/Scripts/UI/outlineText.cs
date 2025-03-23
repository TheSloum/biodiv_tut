using TMPro;
using UnityEngine;

public class OutlineText : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    [Header("Outline Settings")]
    public Color outlineColor = Color.white;
    public float outlineWidth = 0.2f;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        if (textMeshPro == null)
        {
            return;
        }


        textMeshPro.outlineWidth = outlineWidth;
        textMeshPro.outlineColor = outlineColor;
    }
}
