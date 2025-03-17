using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 

public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = Color.blue; 
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = Color.white; 
    }
}
