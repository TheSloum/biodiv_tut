using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // Pour TextMeshProUGUI

public class ResetButtonSelection : MonoBehaviour
{
    public Color normalTextColor = Color.white; // Texte par défaut (blanc)
    public Color hoverTextColor = Color.blue;   // Texte en bleu lors du survol

    private void OnEnable()
    {
        // Réinitialiser la sélection dans l'EventSystem
        EventSystem.current.SetSelectedGameObject(null);

        // Réinitialiser les couleurs de TOUS les boutons TMP dans le menu
        ResetAllButtonColors();
    }

    // Méthode pour réinitialiser tous les boutons du menu
    private void ResetAllButtonColors()
    {
        Button[] allButtons = FindObjectsOfType<Button>();

        foreach (Button button in allButtons)
        {
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.color = normalTextColor;

                // Ajout automatique de l'effet de survol
                AddHoverEffect(button, buttonText);
            }

            // Réinitialisation du bloc de couleurs du bouton
            ColorBlock colors = button.colors;
            colors.normalColor = normalTextColor;
            colors.highlightedColor = hoverTextColor; // Survol en bleu
            colors.pressedColor = hoverTextColor;     // Pressé en bleu
            button.colors = colors;
        }
    }

    // Méthode pour ajouter l'effet de survol dynamiquement
    private void AddHoverEffect(Button button, TextMeshProUGUI buttonText)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();

        // Si le bouton n'a pas encore d'EventTrigger, on l'ajoute
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        // Effet au survol
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) => buttonText.color = hoverTextColor);
        trigger.triggers.Add(pointerEnter);

        // Effet à la sortie du survol
        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) => buttonText.color = normalTextColor);
        trigger.triggers.Add(pointerExit);
    }
}
