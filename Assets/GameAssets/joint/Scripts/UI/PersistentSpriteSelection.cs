using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ce script g�re l'affichage du sprite "actif" lorsque le bouton est s�lectionn�,
// et le sprite "inactif" lorsque le bouton est d�s�lectionn�.
public class PersistentSpriteSelection : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [Header("Sprites � assigner")]
    public Sprite inactiveSprite; // Ton bouton gris
    public Sprite activeSprite;   // Ton bouton bleu

    private Image buttonImage;

    private void Awake()
    {
        // On r�cup�re le composant Image du bouton
        buttonImage = GetComponent<Image>();

        // Au d�part, on affiche le sprite inactif
        if (buttonImage != null)
        {
            buttonImage.sprite = inactiveSprite;
        }
    }

    // Quand l'utilisateur clique sur ce bouton, on force la s�lection dans l'EventSystem
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // Appel� lorsque le bouton devient s�lectionn� par l'EventSystem
    public void OnSelect(BaseEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = activeSprite;
        }
    }

    // Appel� lorsque le bouton perd la s�lection (un autre bouton est s�lectionn�, par ex.)
    public void OnDeselect(BaseEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = inactiveSprite;
        }
    }
}
