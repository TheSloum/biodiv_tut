using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class IsHoveringColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();

        textMeshPro.color = Color.white;

        textMeshPro.outlineWidth = 0.3f;
        textMeshPro.outlineColor = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        textMeshPro.color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMeshPro.color = Color.white;
    }
}
