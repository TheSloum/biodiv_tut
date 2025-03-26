using UnityEngine;
using TMPro;

public class E_EventDescManager : MonoBehaviour
{
    [Header("Références UI")]
    // Le container qui contient les textes (doit avoir le tag "eventdesc")
    public GameObject eventDescContainer;
    // Composant TextMeshPro pour le titre
    public TMP_Text titleText;
    // Composant TextMeshPro pour la description
    public TMP_Text descriptionText;

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
    }

    /// <summary>
    /// Appelée lors du clic sur le bouton d'événement.
    /// Active le container de description et met à jour les textes en fonction de l'événement actif.
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
            titleText.text = "Événement Inconnu";
            descriptionText.text = "Aucune description disponible.";
            return;
        }

        bool found = false;

        // Recherche dans les types d'invasion
        foreach (var invasion in eventSettings.invasionTypes)
        {
            if (invasion.eventID == currentEventID)
            {
                titleText.text = invasion.name;
                descriptionText.text = invasion.description;
                found = true;
                break;
            }
        }

        // Si pas trouvé, recherche dans les événements normaux
        if (!found)
        {
            foreach (var normal in eventSettings.normalEvents)
            {
                if (normal.eventID == currentEventID)
                {
                    titleText.text = normal.name;
                    descriptionText.text = normal.description;
                    found = true;
                    break;
                }
            }
        }

        // Si aucun événement correspondant n'est trouvé
        if (!found)
        {
            titleText.text = "Événement Inconnu";
            descriptionText.text = "Aucune description disponible.";
        }
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
