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

    [Header("Event Definitions")]
    public E_EventDefinition[] invasionEvents;
    public E_EventDefinition[] normalEvents;

    [Header("Scheduling Parameters")]
    [SerializeField] private int invasionCooldownYears = 1;
    [SerializeField] private int normalEventCooldownYears = 3;

    private Queue<E_EventDefinition> invasionQueue = new Queue<E_EventDefinition>();
    private List<E_EventDefinition> availableNormalEvents = new List<E_EventDefinition>();
    private Dictionary<string, int> eventCooldowns = new Dictionary<string, int>();

    private int lastInvasionYear = -1;
    private E_EventDefinition lastInvasionEvent;
    #endregion

    #region Festival du Corail
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
        if (invasionEvents != null && invasionEvents.Length > 0)
        {
            foreach (var inv in invasionEvents)
            {
                invasionQueue.Enqueue(inv);
            }
        }

        if (normalEvents != null)
        {
            availableNormalEvents.AddRange(normalEvents);
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
        E_EventDefinition invasionEvent = invasionQueue.Dequeue();
        invasionQueue.Enqueue(invasionEvent);
        lastInvasionYear = currentYear;
        lastInvasionEvent = invasionEvent;
        int index = GetEventIndex(invasionEvent);
        if (index != -1)
        {
            eventSystem.TriggerEvent(index);
            AddEventCooldown(invasionEvent.eventName, currentYear + invasionCooldownYears);
        }
    }

    void TriggerNormalEvent(int currentYear)
    {
        if (availableNormalEvents.Count == 0)
            return;
        int randomIndex = Random.Range(0, availableNormalEvents.Count);
        E_EventDefinition normalEvent = availableNormalEvents[randomIndex];
        int index = GetEventIndex(normalEvent);
        if (index != -1)
        {
            eventSystem.TriggerEvent(index);
            availableNormalEvents.RemoveAt(randomIndex);
            AddEventCooldown(normalEvent.eventName, currentYear + normalEventCooldownYears);
        }
    }
    #endregion

    #region Gestion du Festival et des Cooldowns
    void CheckForCoralFestival(int currentMonth, int currentYear)
    {
        // Logique à compléter si besoin pour déclencher un festival du corail
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
            foreach (var evt in normalEvents)
            {
                if (evt.eventName == key && !availableNormalEvents.Contains(evt))
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

    #region Méthode Utilitaire
    int GetEventIndex(E_EventDefinition eventDef)
    {
        for (int i = 0; i < eventSystem.eventDefinitions.Length; i++)
        {
            if (eventSystem.eventDefinitions[i] == eventDef)
                return i;
        }
        return -1;
    }
    #endregion
}
