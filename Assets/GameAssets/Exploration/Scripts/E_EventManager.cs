using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class E_EventManager : MonoBehaviour
{
    #region Références et Variables
    [Header("References")]
    public E_Event eventSystem;
    public J_TimeManager timeManager;
    public E_EventSettings eventSettings; // Référence à l'asset de configuration

    [Header("Scheduling Parameters")]
    [SerializeField] private int invasionCooldownYears = 1;
    [SerializeField] private int normalEventCooldownYears = 3;

    // File et liste d'événements planifiés en utilisant les types définis dans l'asset
    private Queue<InvasionType> invasionQueue = new Queue<InvasionType>();
    private List<NormalEventType> availableNormalEvents = new List<NormalEventType>();
    private Dictionary<string, int> eventCooldowns = new Dictionary<string, int>();

    private int lastInvasionYear = -1;
    private InvasionType lastInvasionEvent;
    #endregion

    #region Festival du Corail (optionnel)
    private bool coralFestivalPending = false;
    #endregion

    #region Initialisation et Abonnements
    void Start()
    {
        InitializeEvents();
        SubscribeToTimeEvents();
    }

    void InitializeEvents()
    {
        // Remplissage de la file des invasions à partir des paramètres
        if (eventSettings.invasionTypes != null && eventSettings.invasionTypes.Count > 0)
        {
            foreach (var inv in eventSettings.invasionTypes)
            {
                invasionQueue.Enqueue(inv);
            }
        }

        // Remplissage de la liste des événements normaux à partir des paramètres
        if (eventSettings.normalEvents != null)
        {
            availableNormalEvents.AddRange(eventSettings.normalEvents);
        }
    }

    void SubscribeToTimeEvents()
    {
        timeManager.OnMonthChanged += HandleMonthChange;
        timeManager.OnYearChanged += HandleYearChange;
    }

    void OnDestroy()
    {
        timeManager.OnMonthChanged -= HandleMonthChange;
        timeManager.OnYearChanged -= HandleYearChange;
    }
    #endregion

    #region Gestion des Événements Mensuels et Annuels
    void HandleMonthChange(int month)
    {
        int year = timeManager.GetCurrentYear();
        CheckForScheduledEvents(month, year);
        CheckForCoralFestival(month, year);
    }

    void HandleYearChange(int year)
    {
        CleanupCooldowns(year);
    }
    #endregion

    #region Déclenchement des Événements
    void CheckForScheduledEvents(int currentMonth, int currentYear)
    {
        // Exemple : déclencher une invasion en mars si aucune invasion n'a eu lieu cette année
        if (currentMonth == 3 && currentYear > lastInvasionYear)
        {
            TriggerInvasion(currentYear);
        }
        // Exemple : déclencher un événement normal en juin ou en septembre
        if ((currentMonth == 6 || currentMonth == 9) && availableNormalEvents.Count > 0 && !IsEventInCooldown(currentYear))
        {
            TriggerNormalEvent(currentYear);
        }
    }

    void TriggerInvasion(int currentYear)
    {
        if (invasionQueue.Count == 0)
            return;

        InvasionType invasionEvent = invasionQueue.Dequeue();
        invasionQueue.Enqueue(invasionEvent); // Réinsérer l'événement pour qu'il puisse se reproduire

        lastInvasionYear = currentYear;
        lastInvasionEvent = invasionEvent;

        // Déclenche l'événement avec son ID et la durée spécifique définie dans l'asset
        eventSystem.TriggerEvent(invasionEvent.eventID, invasionEvent.durationInMonths);
        AddEventCooldown(invasionEvent.name, currentYear + invasionCooldownYears);

        // Activation du mode invasion dans le spawner si un prefab est défini
        if (invasionEvent.prefabs != null && invasionEvent.prefabs.Length > 0)
        {
            E_FishSpawner.Instance.EnableInvasionMode(invasionEvent.prefabs[0]);
            Debug.Log("Invasion déclenchée : " + invasionEvent.name);
        }
        else
        {
            Debug.LogWarning("Aucun prefab trouvé pour l'événement d'invasion : " + invasionEvent.name);
        }
    }

    void TriggerNormalEvent(int currentYear)
    {
        if (availableNormalEvents.Count == 0)
            return;

        int randomIndex = Random.Range(0, availableNormalEvents.Count);
        NormalEventType normalEvent = availableNormalEvents[randomIndex];

        // Déclenche l'événement normal avec sa durée spécifique définie dans l'asset
        eventSystem.TriggerEvent(normalEvent.eventID, normalEvent.durationInMonths);
        availableNormalEvents.RemoveAt(randomIndex);
        AddEventCooldown(normalEvent.name, currentYear + normalEventCooldownYears);
    }
    #endregion

    #region Gestion du Festival et des Cooldowns
    void CheckForCoralFestival(int currentMonth, int currentYear)
    {
        // Logique pour le festival du corail si besoin (similaire aux autres scripts)
    }

    void CleanupCooldowns(int currentYear)
    {
        List<string> expired = new List<string>();
        foreach (var entry in eventCooldowns)
        {
            if (entry.Value <= currentYear)
                expired.Add(entry.Key);
        }
        foreach (string key in expired)
        {
            eventCooldowns.Remove(key);
            // Réintégrer l'événement normal correspondant s'il n'est plus en cooldown
            foreach (var evt in eventSettings.normalEvents)
            {
                if (evt.name == key && !availableNormalEvents.Contains(evt))
                {
                    availableNormalEvents.Add(evt);
                }
            }
        }
    }

    void AddEventCooldown(string eventName, int expiryYear)
    {
        if (!eventCooldowns.ContainsKey(eventName))
        {
            eventCooldowns.Add(eventName, expiryYear);
        }
    }

    bool IsEventInCooldown(int currentYear)
    {
        foreach (var entry in eventCooldowns)
        {
            if (entry.Value == currentYear)
                return true;
        }
        return false;
    }
    #endregion
}
