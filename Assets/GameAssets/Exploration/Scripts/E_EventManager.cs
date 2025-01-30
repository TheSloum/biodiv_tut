using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class E_EventManager : MonoBehaviour
{
    [Header("Références")]
    public E_EventSettings settings;
    public E_Event eventSystem;
    public J_TimeManager timeManager;

    [Header("Paramètres des Événements")]
    [SerializeField] private int invasionCooldown = 1; // Années
    [SerializeField] private int normalEventCooldown = 3; // Années

    private Queue<string> invasionQueue = new Queue<string>();
    private List<string> availableNormalEvents = new List<string>();
    private Dictionary<string, int> eventCooldowns = new Dictionary<string, int>();

    private int lastInvasionYear = -1;
    private string lastInvasionType;
    private bool coralFestivalPending;

    void Start()
    {
        InitializeEventSystem();
        SubscribeToTimeEvents();
    }

    void InitializeEventSystem()
    {
        if (eventSystem == null)
            eventSystem = FindObjectOfType<E_Event>();
        
        invasionQueue.Enqueue("Méduses");
        invasionQueue.Enqueue("PoissonLion");
        invasionQueue.Enqueue("Barracuda");
        
        availableNormalEvents.AddRange(new[] { "VagueDechets", "Tempête", "AlguesToxiques" });
    }

    void SubscribeToTimeEvents()
    {
        timeManager.OnMonthChanged += HandleMonthChange;
        timeManager.OnYearChanged += HandleYearChange;
    }

    void HandleMonthChange(int month)
    {
        int year = timeManager.GetCurrentYear();
        CheckForScheduledEvents(month, year);
        CheckCoralFestival(month, year);
    }

    void HandleYearChange(int year)
    {
        CleanupCooldowns(year);
    }

    void CheckForScheduledEvents(int currentMonth, int currentYear)
    {
        // Vérifier les invasions (1 par an)
        if (currentMonth == 3 && currentYear > lastInvasionYear)
        {
            TriggerInvasion(currentYear);
        }

        // Vérifier les événements normaux (2 par an)
        if ((currentMonth == 6 || currentMonth == 9) && 
           availableNormalEvents.Count > 0 && 
           !IsEventInCooldown(currentYear))
        {
            TriggerNormalEvent(currentYear);
        }
    }

    void TriggerInvasion(int currentYear)
    {
        string invasionType = GetNextInvasionType();
        lastInvasionYear = currentYear;
        lastInvasionType = invasionType;

        eventSystem.TriggerEvent(GetEventIdFromType(invasionType));
        AddEventCooldown(invasionType, currentYear + invasionCooldown);
    }

    void TriggerNormalEvent(int currentYear)
    {
        string eventType = availableNormalEvents[Random.Range(0, availableNormalEvents.Count)];
        eventSystem.TriggerEvent(GetEventIdFromType(eventType));
        
        availableNormalEvents.Remove(eventType);
        AddEventCooldown(eventType, currentYear + normalEventCooldown);
    }

    void CheckCoralFestival(int currentMonth, int currentYear)
    {
        if (ShouldTriggerCoralFestival(currentMonth, currentYear))
        {
            if (IsPlayerExploring())
            {
                coralFestivalPending = true;
            }
            else
            {
                StartCoroutine(TriggerDelayedFestival());
            }
        }
    }

    IEnumerator TriggerDelayedFestival()
    {
        yield return new WaitUntil(() => !IsPlayerExploring());
        eventSystem.TriggerEvent(3);
        coralFestivalPending = false;
    }

    bool ShouldTriggerCoralFestival(int month, int year)
    {
        return month == 6 && 
              year % 2 == 0 && 
              !coralFestivalPending && 
              timeManager.GetCurrentTimer() > 3 * 60; // 3 minutes en secondes
    }

    void CleanupCooldowns(int currentYear)
    {
        List<string> expiredEvents = new List<string>();
        
        foreach (var entry in eventCooldowns)
        {
            if (entry.Value <= currentYear)
            {
                expiredEvents.Add(entry.Key);
            }
        }

        foreach (string eventType in expiredEvents)
        {
            eventCooldowns.Remove(eventType);
            if (!availableNormalEvents.Contains(eventType))
            {
                availableNormalEvents.Add(eventType);
            }
        }
    }

    int GetEventIdFromType(string eventType)
    {
        return eventType switch {
            "Méduses" => 0,
            "PoissonLion" => 1,
            "Barracuda" => 2,
            "FêteCorail" => 3,
            "VagueDechets" => 4,
            _ => -1
        };
    }

    string GetNextInvasionType()
    {
        string nextType = invasionQueue.Dequeue();
        invasionQueue.Enqueue(nextType);
        return nextType;
    }

    void AddEventCooldown(string eventType, int expiryYear)
    {
        if (!eventCooldowns.ContainsKey(eventType))
        {
            eventCooldowns.Add(eventType, expiryYear);
        }
    }

    bool IsEventInCooldown(int currentYear)
    {
        return eventCooldowns.ContainsValue(currentYear); 
    }

    bool IsPlayerExploring()
    {
        // Implémentez votre logique de vérification d'exploration ici
        return false;
    }

    void OnDestroy()
    {
        timeManager.OnMonthChanged -= HandleMonthChange;
        timeManager.OnYearChanged -= HandleYearChange;
    }
}