using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Ce script gère l'affichage du sprite "actif" lorsque le bouton est sélectionné,
// et le sprite "inactif" lorsque le bouton est désélectionné.
public class PersistentSpriteSelection : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [Header("Sprites à assigner")]
    public Sprite inactiveSprite; // Ton bouton gris
    public Sprite activeSprite;   // Ton bouton bleu

    private Image buttonImage;

    private void Awake()
    {
        // On récupère le composant Image du bouton
        buttonImage = GetComponent<Image>();

        // Au départ, on affiche le sprite inactif
        if (buttonImage != null)
        {
            buttonImage.sprite = inactiveSprite;
        }
    }

    // Quand l'utilisateur clique sur ce bouton, on force la sélection dans l'EventSystem
    public void OnPointerClick(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    // Appelé lorsque le bouton devient sélectionné par l'EventSystem
    public void OnSelect(BaseEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = activeSprite;
        }
    }

    // Appelé lorsque le bouton perd la sélection (un autre bouton est sélectionné, par ex.)
    public void OnDeselect(BaseEventData eventData)
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = inactiveSprite;
        }
    }
}
