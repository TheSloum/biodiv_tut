using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ImagePressSwap : MonoBehaviour, IPointerDownHandler
{
    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite pressedSprite;

    private Image img;
    private bool isPressed = false;

    private void Awake()
    {
        img = GetComponent<Image>();
        img.sprite = normalSprite;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = pressedSprite;
        isPressed = true;
    }

    private void Update()
    {
        // Quand on relâche le clic (quelque soit la position du curseur)
        if (isPressed && Input.GetMouseButtonUp(0))
        {
            img.sprite = normalSprite;
            isPressed = false;
        }
    }
}
