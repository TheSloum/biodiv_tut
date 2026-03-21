using UnityEngine;
using UnityEngine.UI; // Nécessaire pour le composant Image
using TMPro;

public class E_EventDescManager : MonoBehaviour
{
    [Header("Références UI")]
    // Le container qui contient les textes et l'image (doit avoir le tag "eventdesc")
    public GameObject eventDescContainer;
    // Composant TextMeshPro pour le titre
    public TMP_Text titleText;
    // Composant TextMeshPro pour la description
    public TMP_Text descriptionText;
    // Composant Image pour l'illustration de l'événement
    public Image eventImage; 

    [Header("Références aux Settings")]
    // L'asset de configuration des événements
    public E_EventSettings eventSettings;

    private void Awake()
    {
        // Si le container n'est pas assigné, essayer de le trouver par son tag
        if (eventDescContainer == null)
        {
            eventDescContainer = GameObject.FindWithTag("eventdesc");
            if (eventDescContainer == null)
                Debug.LogWarning("Aucun GameObject avec le tag 'eventdesc' n'a été trouvé !");
        }

        // Optionnel : S'assurer que l'image est bien assignée si le container existe
        if (eventDescContainer != null && eventImage == null)
        {
            // Essaie de trouver le composant Image dans les enfants du container
            eventImage = eventDescContainer.GetComponentInChildren<Image>();
        }
    }

    /// <summary>
    /// Appelée lors du clic sur le bouton d'événement.
    /// Active le container de description et met à jour les textes et l'image en fonction de l'événement actif.
    /// </summary>
    public void ShowEventDescription()
    {
        if (eventDescContainer == null)
        {
            Debug.LogWarning("eventDescContainer non assigné.");
            return;
        }

        eventDescContainer.SetActive(true);

        // Récupérer l'ID de l'événement actif depuis le singleton E_Event
        int currentEventID = E_Event.activeEventID;

        if (currentEventID == -1)
        {
            Debug.LogWarning("Aucun événement actif pour afficher la description.");
            SetUnknownEventUI();
            return;
        }

        bool found = false;

        // --- Recherche dans les types d'invasion ---
        if (eventSettings.invasionTypes != null)
        {
            foreach (var invasion in eventSettings.invasionTypes)
            {
                if (invasion.eventID == currentEventID)
                {
                    UpdateUI(invasion.name, invasion.description, invasion.eventIcon); // Supposant que vous ajoutez 'eventIcon' dans votre classe de données
                    found = true;
                    break;
                }
            }
        }

        // --- Si pas trouvé, recherche dans les événements normaux ---
        if (!found && eventSettings.normalEvents != null)
        {
            foreach (var normal in eventSettings.normalEvents)
            {
                if (normal.eventID == currentEventID)
                {
                    UpdateUI(normal.name, normal.description, normal.eventIcon); // Supposant que vous ajoutez 'eventIcon' dans votre classe de données
                    found = true;
                    break;
                }
            }
        }

        // Si aucun événement correspondant n'est trouvé dans les settings
        if (!found)
        {
            Debug.LogWarning($"Événement avec l'ID {currentEventID} non trouvé dans les settings.");
            SetUnknownEventUI();
        }
    }

    /// <summary>
    /// Met à jour les éléments UI (Titre, Description, Image).
    /// Gère le cas où le Sprite est null.
    /// </summary>
    private void UpdateUI(string title, string description, Sprite icon)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = description;

        if (eventImage != null)
        {
            if (icon != null)
            {
                eventImage.sprite = icon;
                eventImage.enabled = true; // Active le composant Image s'il y a un sprite
            }
            else
            {
                // Si pas d'image définie dans les settings, on cache le composant pour éviter le carré blanc
                eventImage.enabled = false; 
            }
        }
    }

    /// <summary>
    /// Définit l'UI pour un événement inconnu ou une erreur.
    /// </summary>
    private void SetUnknownEventUI()
    {
        UpdateUI("Événement Inconnu", "Aucune description disponible.", null);
    }

    /// <summary>
    /// Appelée lors du clic sur le GameObject "croix" pour fermer la description.
    /// Désactive le container de description.
    /// </summary>
    public void HideEventDescription()
    {
        Debug.Log("HideEventDescription() called.");
        if (eventDescContainer != null)
        {
            eventDescContainer.SetActive(false);
            Debug.Log("eventDescContainer désactivé.");
        }
        else
        {
            Debug.LogWarning("eventDescContainer est null lors de l'appel de HideEventDescription.");
        }
    }
}