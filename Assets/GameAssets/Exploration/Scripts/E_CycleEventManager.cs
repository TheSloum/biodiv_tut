using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class E_CycleEventManager : MonoBehaviour
{
    #region Références et Variables
    [Header("Références")]
    [Tooltip("Référence au script E_Event qui déclenche concrètement les événements")]
    public E_Event eventSystem;

    [Tooltip("Référence au TimeManager pour suivre le temps (jour/mois/année)")]
    public J_TimeManager timeManager;

    [Tooltip("Référence aux paramètres des événements (ScriptableObject)")]
    public E_EventSettings eventSettings;

    // Gestion des cooldowns et historiques
    private Dictionary<int, int> normalEventCooldowns = new Dictionary<int, int>();
    private int lastInvasionEventID = -1;
    private List<(int year, int eventID)> invasionHistory = new List<(int, int)>();

    // File d'événements planifiés pour l'année en cours
    private Queue<ScheduledEvent> yearlyEventQueue = new Queue<ScheduledEvent>();

    // Flags et état de l'événement en cours
    private bool coralFestivalPending = false;
    private bool coralFestivalDoneThisYear = false;
    private bool waitingNoEventCooldown = false;
    private bool isEventActive = false;
    private ScheduledEvent currentEvent = null;
    private (int year, int month) eventEndDate;

    // Pour éviter de remplir plusieurs fois la file pour une même année
    private int lastFilledQueueYear = -1;
    #endregion

    #region Singleton
    private static E_CycleEventManager _instance;
    public static E_CycleEventManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persistance entre scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Initialisation et Abonnement aux Événements Temps
    void Start()
    {
        if (timeManager == null)
            timeManager = FindObjectOfType<J_TimeManager>();
        if (eventSystem == null)
            eventSystem = FindObjectOfType<E_Event>();
        if (eventSettings == null)
        {
            Debug.LogError("E_EventSettings n'est pas assigné dans E_CycleEventManager.");
            return;
        }

        InitializeEventSystem();

        // Abonnement aux callbacks de temps
        timeManager.OnDayChanged += OnDayChanged;
        timeManager.OnMonthChanged += OnMonthChanged;
        timeManager.OnYearChanged  += OnYearChanged;

        // Remplissage de la file pour l'année en cours
        FillYearlyEventQueue(timeManager.GetCurrentYear());
    }

    void OnDestroy()
    {
        if (timeManager != null)
        {
            timeManager.OnDayChanged -= OnDayChanged;
            timeManager.OnMonthChanged -= OnMonthChanged;
            timeManager.OnYearChanged  -= OnYearChanged;
        }
    }

    void InitializeEventSystem()
    {
        lastInvasionEventID = -1;
        invasionHistory.Clear();
        normalEventCooldowns.Clear();
        Debug.Log("Système d'événements initialisé.");
    }
    #endregion

    #region Gestion des Événements liés au Temps
    void OnDayChanged(int newDay, int newMonth)
    {
        if (!isEventActive && !waitingNoEventCooldown)
        {
            if (newDay > eventSettings.noEventStartDays && newDay < (timeManager.daysPerMonth - eventSettings.noEventEndDays))
            {
                TryStartCycleEvent();
            }
        }
    }

    void OnMonthChanged(int newMonth)
    {
        int currentYear = timeManager.GetCurrentYear();
        if (newMonth == eventSettings.coralFestivalMonth && !coralFestivalDoneThisYear)
        {
            TriggerCoralFestivalIfNeeded(currentYear);
        }
    }

    void OnYearChanged(int newYear)
    {
        coralFestivalDoneThisYear = false;
        FillYearlyEventQueue(newYear);
        CleanupNormalEventCooldowns(newYear);
        CleanupInvasionHistory(newYear);
    }
    #endregion

    #region Planification de la File d'Événements
    void FillYearlyEventQueue(int year)
    {
        if (lastFilledQueueYear == year)
        {
            Debug.Log($"La file a déjà été remplie pour l'année {year}.");
            return;
        }
        lastFilledQueueYear = year;
        yearlyEventQueue.Clear();

        int invasionsThisYear = eventSettings.invasionsPerYear;
        int normalEventsThisYear = eventSettings.normalEventsPerYear;

        Debug.Log($"Remplissage de la file pour l'année {year} : {invasionsThisYear} invasions et {normalEventsThisYear} événements normaux.");

        // Planifier les invasions
        for (int i = 0; i < invasionsThisYear; i++)
        {
            int invasionID = GetNextInvasionID(year);
            if (invasionID != -1)
            {
                ScheduledEvent sched = new ScheduledEvent { type = EventType.Invasion, eventID = invasionID, name = GetEventNameByID(invasionID) };
                yearlyEventQueue.Enqueue(sched);
                Debug.Log($"Enfilage d'une invasion : ID {invasionID}, {sched.name}");
            }
            else
            {
                Debug.LogWarning("Aucune invasion disponible pour cette année.");
            }
        }

        // Planifier les événements normaux
        for (int i = 0; i < normalEventsThisYear; i++)
        {
            int normalID = GetNextNormalEventID(year);
            if (normalID != -1)
            {
                ScheduledEvent sched = new ScheduledEvent { type = EventType.Normal, eventID = normalID, name = GetEventNameByID(normalID) };
                yearlyEventQueue.Enqueue(sched);
                Debug.Log($"Enfilage d'un événement normal : ID {normalID}, {sched.name}");
            }
            else
            {
                Debug.LogWarning("Aucun événement normal disponible pour cette année.");
            }
        }

        // Planifier la Fête du Corail tous les 2 ans
        if (year % 2 == 0)
        {
            ScheduledEvent sched = new ScheduledEvent { type = EventType.CoralFestival, eventID = 3, name = "Fête du Corail" };
            yearlyEventQueue.Enqueue(sched);
            Debug.Log("Enfilage de la Fête du Corail.");
        }

        Debug.Log($"File annuelle complétée pour l'année {year} avec {yearlyEventQueue.Count} événements.");
    }
    #endregion

    #region Déclenchement des Événements
    void TryStartCycleEvent()
    {
        if (yearlyEventQueue.Count == 0)
        {
            Debug.Log("La file est vide, aucun événement à démarrer.");
            return;
        }
        ScheduledEvent next = yearlyEventQueue.Dequeue();
        currentEvent = next;
        Debug.Log($"Début de l'événement : {next.name} (Type: {next.type})");

        int eventDurationMonths = (next.type == EventType.CoralFestival)
            ? eventSettings.coralFestivalDuration
            : Random.Range(2, 5); // 2, 3 ou 4 mois

        // Calcul de la date de fin de l'événement
        (int endYear, int endMonth) = CalculateEventEndDate(timeManager.GetCurrentYear(), timeManager.GetCurrentMonth(), eventDurationMonths);
        eventEndDate = (endYear, endMonth);
        Debug.Log($"Date de fin de l'événement : Année {endYear}, Mois {endMonth}");

        StartCoroutine(StartEvent(next.eventID, eventDurationMonths, next.type));
    }

    (int, int) CalculateEventEndDate(int startYear, int startMonth, int durationMonths)
    {
        int endMonth = startMonth + durationMonths;
        int endYear = startYear;
        while (endMonth > 12)
        {
            endMonth -= 12;
            endYear++;
        }
        return (endYear, endMonth);
    }

    IEnumerator StartEvent(int eventID, int durationMonths, EventType type)
    {
        isEventActive = true;
        eventSystem.TriggerEvent(eventID);
        Debug.Log($"Événement déclenché : {GetEventNameByID(eventID)}, durée {durationMonths} mois");

        if (type == EventType.Invasion)
        {
            // Activation du mode invasion sur le spawner
            TriggerInvasion(eventID);
            int currentYear = timeManager.GetCurrentYear();
            invasionHistory.Add((currentYear, eventID));
            lastInvasionEventID = eventID;
        }

        if (type == EventType.Invasion)
        {
            // Attendre que la date ingame atteigne (ou dépasse) la date de fin de l'événement
            yield return new WaitUntil(() => {
                int currentYear = timeManager.GetCurrentYear();
                int currentMonth = timeManager.GetCurrentMonth();
                return (currentYear > eventEndDate.Item1) || (currentYear == eventEndDate.Item1 && currentMonth >= eventEndDate.Item2);
            });
        }
        else
        {
            float realDuration = (type == EventType.CoralFestival)
                ? eventSettings.coralFestivalDuration * 300f
                : durationMonths * 300f;
            yield return new WaitForSeconds(realDuration);
        }

        if (type == EventType.Invasion)
        {
            // Fin de l'invasion : rétablir le spawn normal
            E_FishSpawner.Instance.DisableInvasionMode();
        }

        isEventActive = false;
        currentEvent = null;
        Debug.Log($"Événement terminé : {GetEventNameByID(eventID)}");

        waitingNoEventCooldown = true;
        yield return new WaitForSeconds(120f);
        waitingNoEventCooldown = false;
    }
    #endregion

    #region Gestion de la Fête du Corail
    void TriggerCoralFestivalIfNeeded(int currentYear)
    {
        if (eventSettings.waitForExplorationIfCoralFestival && IsPlayerExploring())
        {
            coralFestivalPending = true;
            StartCoroutine(WaitEndExplorationAndTriggerCoral());
        }
        else
        {
            eventSystem.TriggerEvent(3);
            coralFestivalDoneThisYear = true;
            Debug.Log("Fête du Corail déclenchée directement.");
        }
    }

    IEnumerator WaitEndExplorationAndTriggerCoral()
    {
        yield return new WaitUntil(() => !IsPlayerExploring());
        eventSystem.TriggerEvent(3);
        coralFestivalDoneThisYear = true;
        Debug.Log("Fête du Corail déclenchée après exploration.");
    }

    bool IsPlayerExploring()
    {
        return SceneManager.GetActiveScene().name == "Exploration_main";
    }
    #endregion

    #region Méthodes Utilitaires pour la Sélection et les Cooldowns
    int GetNextInvasionID(int currentYear)
    {
        List<int> allInvasions = eventSettings.invasionTypes.Select(inv => inv.eventID).ToList();
        List<int> required = new List<int>();
        foreach (int invID in allInvasions)
        {
            bool found = invasionHistory.Any(entry => entry.eventID == invID && entry.year > currentYear - 3);
            if (!found)
                required.Add(invID);
        }

        List<int> candidatePool = required.Count > 0 ? required : allInvasions.Where(id => id != lastInvasionEventID).ToList();

        return candidatePool.Count == 0 ? -1 : candidatePool[Random.Range(0, candidatePool.Count)];
    }

    int GetNextNormalEventID(int currentYear)
    {
        List<int> available = eventSettings.normalEvents
            .Select(e => e.eventID)
            .Where(id => !normalEventCooldowns.ContainsKey(id) || normalEventCooldowns[id] <= currentYear)
            .ToList();

        if (available.Count == 0)
            return -1;

        int chosen = available[Random.Range(0, available.Count)];
        normalEventCooldowns[chosen] = currentYear + 5; // Cooldown de 5 ans
        return chosen;
    }

    void CleanupNormalEventCooldowns(int currentYear)
    {
        var keysToRemove = normalEventCooldowns.Where(kvp => kvp.Value <= currentYear)
                                               .Select(kvp => kvp.Key)
                                               .ToList();
        foreach (int key in keysToRemove)
        {
            normalEventCooldowns.Remove(key);
            Debug.Log($"Cooldown terminé pour l'événement normal ID {key}");
        }
    }

    void CleanupInvasionHistory(int currentYear)
    {
        invasionHistory = invasionHistory.Where(entry => entry.year > currentYear - 3).ToList();
    }

    void TriggerInvasion(int invasionID)
    {
        var invasion = eventSettings.invasionTypes.FirstOrDefault(inv => inv.eventID == invasionID);
        if (invasion != null && invasion.prefabs.Length > 0)
        {
            GameObject invasionPrefab = invasion.prefabs[0];
            E_FishSpawner.Instance.EnableInvasionMode(invasionPrefab);
            Debug.Log($"Invasion déclenchée : {invasion.name} (ID: {invasionID}) - Mode invasion activé.");
        }
        else
        {
            Debug.LogWarning($"Aucune prefab trouvée pour l'invasion ID {invasionID}.");
        }
    }

    string GetEventNameByID(int eventID)
    {
        if (eventID == 3)
            return "Fête du Corail";

        var invasion = eventSettings.invasionTypes.FirstOrDefault(inv => inv.eventID == eventID);
        if (invasion != null)
            return invasion.name;

        var normalEvent = eventSettings.normalEvents.FirstOrDefault(e => e.eventID == eventID);
        if (normalEvent != null)
            return normalEvent.name;

        return "Événement inconnu";
    }
    #endregion

    #region Classes Internes et Enumérations
    private enum EventType { Normal, Invasion, CoralFestival }

    private class ScheduledEvent
    {
        public EventType type;
        public int eventID;
        public string name;
    }
    #endregion
}
