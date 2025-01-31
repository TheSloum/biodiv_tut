using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Gère la logique des cycles et des events en suivant les règles données.
/// À brancher avec J_TimeManager et E_Event.
/// </summary>
public class E_CycleEventManager : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence au script E_Event (système qui déclenche concrètement les events)")]
    public E_Event eventSystem;

    [Tooltip("Référence au TimeManager pour suivre mois/années")]
    public J_TimeManager timeManager;

    [Header("Settings")]
    [Tooltip("Référence au ScriptableObject contenant les paramètres des events")]
    public E_EventSettings eventSettings;

    // Gestion des cooldowns
    private Dictionary<int, int> eventCooldowns = new Dictionary<int, int>(); // eventID -> (année min)
    private int lastInvasionYear = -1;
    private bool coralFestivalPending;

    // Gestion des événements
    private Queue<int> invasionQueue = new Queue<int>();
    private List<int> availableNormalEventIDs = new List<int>();

    // Pour traquer la "période sans event" suite à un event (12 jours) => 2 minutes
    private bool waitingNoEventCooldown = false;

    // Indique si un event est en cours
    private bool isEventActive = false;
    private EventType currentEventType = EventType.Normal;

    // Stocke quand l'event actuel se termine (en mois + année)
    private (int year, int month) eventEndDate;

    // Indique si la fête du corail a déjà été déclenchée cette année
    private bool coralFestivalDoneThisYear = false;

    // Pour éviter de remplir la queue plus d'une fois par an
    private int lastFilledQueueYear = -1;

    // Ajout d'une variable pour stocker l'événement en cours
    private ScheduledEvent currentEvent;

    // Représente nos types d'event
    private enum EventType
    {
        Normal,
        Invasion,
        CoralFestival
    }

    // Petite classe utilitaire pour stocker un event dans une queue
    private class ScheduledEvent
    {
        public EventType type;
        public int eventID;     // ID utilisé par E_Event pour .TriggerEvent()
        public string name;     // juste pour debug
    }

    private static E_CycleEventManager _instance;
    public static E_CycleEventManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Rend ce GameObject immortel
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (!timeManager) timeManager = FindObjectOfType<J_TimeManager>();
        if (!eventSystem) eventSystem = FindObjectOfType<E_Event>();

        if (!eventSettings)
        {
            Debug.LogError("E_EventSettings n'est pas assigné dans E_CycleEventManager.");
            return;
        }

        // Initialisation des queues d'invasion et des événements normaux
        InitializeEventSystem();

        // On s'abonne aux events du timeManager
        timeManager.OnDayChanged += OnDayChanged; // Ajouté selon les suggestions précédentes
        timeManager.OnMonthChanged += OnMonthChanged;
        timeManager.OnYearChanged  += OnYearChanged;

        // Initialisation
        FillYearlyEventQueue(timeManager.GetCurrentYear());
    }

    void InitializeEventSystem()
    {
        // Enqueue toutes les invasions disponibles
        foreach (var invasion in eventSettings.invasionTypes)
        {
            invasionQueue.Enqueue(invasion.eventID);
        }

        // Enqueue toutes les événements normaux disponibles
        foreach (var normalEvent in eventSettings.normalEvents)
        {
            availableNormalEventIDs.Add(normalEvent.eventID);
        }

        // Log initial queues
        Debug.Log("Initialisation du système d'événements terminée.");
        LogEventQueue();
    }

    void OnDestroy()
    {
        if (timeManager)
        {
            timeManager.OnDayChanged -= OnDayChanged;
            timeManager.OnMonthChanged -= OnMonthChanged;
            timeManager.OnYearChanged  -= OnYearChanged;
        }
    }

    // ===================================================================================================
    // EVENTS TIME MANAGER
    // ===================================================================================================
    private void OnMonthChanged(int newMonth)
    {
        // Calcul de l'index du cycle actuel (ex : mois 1-4 = cycle 0, moisPerCycle=4, etc.)
        int currentYear = timeManager.GetCurrentYear();
        int currentCycleIndex = (newMonth - 1) / eventSettings.monthsPerCycle;  
        
        // Check Fête du Corail
        if (newMonth == eventSettings.coralFestivalMonth && !coralFestivalDoneThisYear)
        {
            TriggerCoralFestivalIfNeeded(currentYear);
        }

        // Convertir le jour actuel en "jour du mois"
        int currentDay = timeManager.currentDay;

        if (!isEventActive && !waitingNoEventCooldown)
        {
            // Vérif si on est hors noEventStartDays ET hors noEventEndDays
            if (currentDay > eventSettings.noEventStartDays && currentDay < (timeManager.daysPerMonth - eventSettings.noEventEndDays))
            {
                // On tente de déclencher un event si un cycle "doit" en avoir un
                TryStartCycleEvent();
            }
        }
    }

    private void OnDayChanged(int newDay, int newMonth)
    {
        int currentYear = timeManager.GetCurrentYear();
        int currentCycleIndex = (newMonth - 1) / eventSettings.monthsPerCycle;  
        
        // Check Fête du Corail
        if (newMonth == eventSettings.coralFestivalMonth && !coralFestivalDoneThisYear)
        {
            TriggerCoralFestivalIfNeeded(currentYear);
        }

        // Conditions pour déclencher un événement
        if (!isEventActive && !waitingNoEventCooldown)
        {
            // Vérif si on est hors noEventStartDays ET hors noEventEndDays
            if (newDay > eventSettings.noEventStartDays && newDay < (timeManager.daysPerMonth - eventSettings.noEventEndDays))
            {
                TryStartCycleEvent();
            }
        }
    }

    private void OnYearChanged(int newYear)
    {
        // Réinitialise la fête du corail
        coralFestivalDoneThisYear = false;

        // On remplit la queue chaque année (pour 1 an de plus)
        FillYearlyEventQueue(newYear);

        // Met à jour les cooldowns
        UpdateEventCooldowns(newYear);
    }

    // ===================================================================================================
    // FONCTIONS PRINCIPALES LOGIQUES
    // ===================================================================================================

    /// <summary>
    /// Remplit la queue d'events annuels (1 an = 3 cycles).
    /// On doit avoir 1 invasion, 2 normal, etc.  
    /// On évite 2 invasions d'affilée d'une année sur l'autre.
    /// Basé sur ton pseudo-code.
    /// </summary>
    private void FillYearlyEventQueue(int year)
    {
        if (lastFilledQueueYear == year)
        {
            Debug.Log($"FillYearlyEventQueue: La queue a déjà été remplie pour l'année {year}.");
            return; // Évite de remplir deux fois la même année
        }

        lastFilledQueueYear = year;
        Debug.Log($"FillYearlyEventQueue: Remplissage de la queue d'événements pour l'année {year}.");

        // On s'assure d'avoir les bons nombres d'événements par an
        int invasionsThisYear = eventSettings.invasionsPerYear;
        int normalEventsThisYear = eventSettings.normalEventsPerYear;

        Debug.Log($"FillYearlyEventQueue: {invasionsThisYear} invasions et {normalEventsThisYear} événements normaux seront ajoutés.");

        // Enqueue les invasions
        for (int i = 0; i < invasionsThisYear; i++)
        {
            int invasionID = GetNextInvasionID(year);
            if (invasionID != -1)
            {
                var sched = new ScheduledEvent { type = EventType.Invasion, eventID = invasionID, name = GetEventNameByID(invasionID) };
                yearlyEventQueue.Enqueue(sched);
                Debug.Log($"FillYearlyEventQueue: Enfilage d'une invasion - ID: {invasionID}, Nom: {sched.name}");
            }
            else
            {
                Debug.LogWarning($"FillYearlyEventQueue: Aucun ID d'invasion disponible pour l'année {year}.");
            }
        }

        // Enqueue les événements normaux
        for (int i = 0; i < normalEventsThisYear; i++)
        {
            int normalEventID = GetNextNormalEventID(year);
            if (normalEventID != -1)
            {
                var sched = new ScheduledEvent { type = EventType.Normal, eventID = normalEventID, name = GetEventNameByID(normalEventID) };
                yearlyEventQueue.Enqueue(sched);
                Debug.Log($"FillYearlyEventQueue: Enfilage d'un événement normal - ID: {normalEventID}, Nom: {sched.name}");
            }
            else
            {
                Debug.LogWarning($"FillYearlyEventQueue: Aucun ID d'événement normal disponible pour l'année {year}.");
            }
        }

        // Ajouter la fête du corail si applicable
        if (year % 2 == 0)
        {
            var sched = new ScheduledEvent { type = EventType.CoralFestival, eventID = 3, name = "Fête du Corail" };
            yearlyEventQueue.Enqueue(sched);
            Debug.Log($"FillYearlyEventQueue: Enfilage de la Fête du Corail pour l'année {year}.");
        }

        Debug.Log($"FillYearlyEventQueue: Total d'événements enfilés pour l'année {year}: {yearlyEventQueue.Count}");
        LogEventQueue();
    }


    /// <summary>
    /// Lorsqu'un nouveau cycle commence, on pioche dans la queue d'events annuels.
    /// Puis on détermine la durée de l'event.
    /// Puis on planifie le déclenchement effectif (si on est hors zone "no-event", etc.).
    /// </summary>
    private void TryStartCycleEvent()
    {
        Debug.Log("TryStartCycleEvent called");
        
        if (yearlyEventQueue.Count == 0)
        {
            Debug.Log("Event queue is empty, returning");
            return;
        }

        // On pop un event
        ScheduledEvent next = yearlyEventQueue.Dequeue();
        currentEvent = next; // Stockage de l'événement en cours
        currentEventType = next.type;
        Debug.Log($"Dequeued event: Type={next.type}, ID={next.eventID}, Nom={next.name}");
        LogEventQueue(); // Afficher la queue après défilement

        // Calcul d'une durée (en mois) si ce n'est pas la fête du corail
        int eventDurationMonths = 0;
        if (next.type == EventType.CoralFestival)
        {
            eventDurationMonths = eventSettings.coralFestivalDuration;
            Debug.Log($"Coral Festival duration set to {eventDurationMonths} months");
        }
        else if (next.type == EventType.Invasion)
        {
            eventDurationMonths = Random.Range((int)eventSettings.eventDurationRange.x, (int)eventSettings.eventDurationRange.y + 1);
            Debug.Log($"Invasion duration set to {eventDurationMonths} months");
        }
        else
        {
            eventDurationMonths = Random.Range((int)eventSettings.eventDurationRange.x, (int)eventSettings.eventDurationRange.y + 1);
            Debug.Log($"Normal event duration set to {eventDurationMonths} months");
        }

        // On calcule la date de fin (mois + année)
        int startMonth = timeManager.GetCurrentMonth();
        int startYear = timeManager.GetCurrentYear();
        Debug.Log($"Event start date: Month {startMonth}, Year {startYear}");

        int endMonth = startMonth + eventDurationMonths;
        int endYear = startYear;
        while (endMonth > 12)
        {
            endMonth -= 12;
            endYear++;
        }

        eventEndDate = (endYear, endMonth);
        Debug.Log($"Event end date calculated: Month {endMonth}, Year {endYear}");

        // On déclenche l'event
        Debug.Log($"Starting event coroutine with ID={next.eventID}, Duration={eventDurationMonths}, Type={next.type}");
        StartCoroutine(StartEvent(next.eventID, eventDurationMonths, next.type));
    }

    /// <summary>
    /// Coroutine qui gère l'event en cours : déclenche, attend la fin, applique la règle
    /// "2 minutes/12 jours de temps ou il ne peut pas avoir d'event" après la fin, etc.
    /// </summary>
    private IEnumerator StartEvent(int eventID, int eventDurationMonths, EventType eventType)
    {
        isEventActive = true;
        
        // Déclenche "réellement" via E_Event
        eventSystem.TriggerEvent(eventID);
        Debug.Log($"Event started: ID={eventID}, Type={eventType}, Name={GetEventNameByID(eventID)}");

        // Gestion spécifique selon le type d'événement
        switch (eventType)
        {
            case EventType.Invasion:
                TriggerInvasion(eventID);
                break;
            case EventType.Normal:
                // Vous pouvez ajouter des comportements spécifiques pour les événements normaux si nécessaire
                break;
            case EventType.CoralFestival:
                // Comportement spécifique pour la fête du corail
                break;
        }

        // Calculer la durée réelle en fonction du temps simulé
        float realDuration = eventDurationMonths * 300f;

        if (eventType == EventType.CoralFestival)
        {
            realDuration = eventSettings.coralFestivalDuration * 300f;
        }

        yield return new WaitForSeconds(realDuration);

        // L'event est terminé
        isEventActive = false;
        currentEvent = null; // Réinitialisation de l'événement en cours
        Debug.Log($"Event ended: ID={eventID}, Type={eventType}, Name={GetEventNameByID(eventID)}");

        // Appliquer le cooldown global
        waitingNoEventCooldown = true;
        yield return new WaitForSeconds(120f);
        waitingNoEventCooldown = false;
    }

    /// <summary>
    /// Déclenche la fête du corail si on est au mois = coralFestivalMonth et si c'est tous les 2 ans
    /// (d'après tes règles, "au milieu du 2e cycle tous les 2 ans"). 
    /// Ici on simplifie un peu la condition de la "2e année" => year%2==0
    /// </summary>
    private void TriggerCoralFestivalIfNeeded(int currentYear)
    {
        if (eventSettings.waitForExplorationIfCoralFestival && IsPlayerExploring())
        {
            coralFestivalPending = true;
            StartCoroutine(WaitEndExplorationAndTriggerCoral());
        }
        else
        {
            // On force directement l'event "3" = Fête du Corail
            eventSystem.TriggerEvent(3);
            coralFestivalDoneThisYear = true;
            Debug.Log("Fête du Corail déclenchée directement.");
        }
    }

    private IEnumerator WaitEndExplorationAndTriggerCoral()
    {
        // On attend que le joueur ne soit plus en exploration
        yield return new WaitUntil(() => !IsPlayerExploring());
        // On déclenche la fête
        eventSystem.TriggerEvent(3);
        coralFestivalDoneThisYear = true;
        Debug.Log("Fête du Corail déclenchée après exploration.");
    }

    /// <summary>
    /// Vérifie si le joueur est en exploration en vérifiant si la scène active est "Exploration_main".
    /// </summary>
    private bool IsPlayerExploring()
    {
        // Vérifie si la scène active est "Exploration_main"
        return SceneManager.GetActiveScene().name == "Exploration_main";
    }

    // ===================================================================================================
    // PICKERS D'ID (pour normal / invasion) + COOLDOWNS
    // ===================================================================================================

    /// <summary>
    /// Choisit un ID d'invasion (évite de prendre deux fois d'affilée la même, 
    /// et assure que sur 3 ans, on aura toutes les invasions au moins une fois).
    /// </summary>
    private int GetNextInvasionID(int currentYear)
    {
        // Filtre la liste d'invasions possible
        var possible = eventSettings.invasionTypes
            .Where(inv => inv.eventID != lastInvasionYear) // Assure que ce n'est pas la même invasion que la précédente
            .Select(inv => inv.eventID)
            .ToList();

        if (possible.Count == 0)
            return -1;

        // On prend aléatoirement
        int chosen = possible[Random.Range(0, possible.Count)];
        
        // Stock
        lastInvasionYear = chosen;
        AddEventCooldown(chosen, currentYear + eventSettings.invasionCooldownYears);
        
        return chosen;
    }

    /// <summary>
    /// Choisit un ID d'event normal parmi ceux disponibles,
    /// en respectant la règle : "Une fois qu'un event normal est arrivé, 
    /// il doit attendre min X ans avant de pouvoir revenir".
    /// </summary>
    private int GetNextNormalEventID(int currentYear)
    {
        // Filtre ceux dont la "nextAvailableYear" est <= currentYear
        var freeEvents = eventSettings.normalEvents
            .Where(e => !eventCooldowns.ContainsKey(e.eventID) || eventCooldowns[e.eventID] <= currentYear)
            .Select(e => e.eventID)
            .ToList();

        if (freeEvents.Count == 0)
        {
            // Si on n'en a pas, on force un par défaut (ou on ignore, à toi de voir)
            return -1;
        }

        int chosen = freeEvents[Random.Range(0, freeEvents.Count)];

        // On set le prochain retour possible
        AddEventCooldown(chosen, currentYear + eventSettings.normalEventCooldownYears);

        return chosen;
    }

    /// <summary>
    /// Ajoute un cooldown pour un événement.
    /// </summary>
    private void AddEventCooldown(int eventID, int expiryYear)
    {
        if (!eventCooldowns.ContainsKey(eventID))
        {
            eventCooldowns.Add(eventID, expiryYear);
        }
        else
        {
            eventCooldowns[eventID] = expiryYear;
        }
    }

    /// <summary>
    /// Met à jour les cooldowns en retirant ceux qui sont expirés.
    /// </summary>
    private void UpdateEventCooldowns(int newYear)
    {
        List<int> toRemove = new List<int>();
        foreach (var kvp in eventCooldowns)
        {
            if (kvp.Value <= newYear)
                toRemove.Add(kvp.Key);
        }

        foreach (var id in toRemove)
        {
            eventCooldowns.Remove(id);
            Debug.Log($"UpdateEventCooldowns: Cooldown terminé pour l'événement ID={id}.");
        }
    }

    /// <summary>
    /// Récupère le nom de l'événement en fonction de l'ID.
    /// </summary>
    private string GetEventNameByID(int eventID)
    {
        if (eventID == 3)
            return "Fête du Corail";

        var invasion = eventSettings.invasionTypes.FirstOrDefault(inv => inv.eventID == eventID);
        if (invasion != null)
            return invasion.name;

        var normalEvent = eventSettings.normalEvents.FirstOrDefault(e => e.eventID == eventID);
        if (normalEvent != null)
            return normalEvent.name;

        return "Unknown Event";
    }

    // ===================================================================================================
    // UTILITAIRES
    // ===================================================================================================

    /// <summary>
    /// Remplissage de la queue annuelle
    /// </summary>
    private Queue<ScheduledEvent> yearlyEventQueue = new Queue<ScheduledEvent>();

    /// <summary>
    /// Méthode pour afficher le contenu actuel de la queue d'événements.
    /// </summary>
    private void LogEventQueue()
    {
        if (yearlyEventQueue.Count == 0)
        {
            Debug.Log("Current Event Queue: (Vide)");
            return;
        }

        var queueList = yearlyEventQueue.Select(e => $"{e.name} (ID: {e.eventID})").ToList();
        Debug.Log("Current Event Queue: " + string.Join(", ", queueList));
    }

    // ===================================================================================================
    // EVENT TRIGGERING UTILITIES
    // ===================================================================================================

    /// <summary>
    /// Déclenche une invasion en instanciant les préfabriqués appropriés.
    /// </summary>
    private void TriggerInvasion(int invasionID)
    {
        var invasion = eventSettings.invasionTypes.FirstOrDefault(inv => inv.eventID == invasionID);
        if (invasion != null && invasion.prefabs.Length > 0)
        {
            // Instancier les préfabriqués d'invasion
            foreach (var prefab in invasion.prefabs)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
                // Ajustez la position et la rotation selon vos besoins
            }
            Debug.Log($"Invasion déclenchée: {invasion.name} (ID: {invasionID})");
        }
        else
        {
            Debug.LogWarning($"TriggerInvasion: Aucune prefab trouvée pour l'invasion ID={invasionID}.");
        }
    }

    /// <summary>
    /// Récupère le nom de l'événement en fonction de l'ID.
    /// </summary>
    private string GetEventName(int eventID)
    {
        return GetEventNameByID(eventID);
    }

    void Update()
    {
        // Vérifier si la touche Y est pressée
        if (Input.GetKeyDown(KeyCode.Y))
        {
            LogCurrentEvent();
        }
    }

    private void LogCurrentEvent()
    {
        if (isEventActive && currentEvent != null)
        {
            Debug.Log($"Événement en cours : {currentEvent.name} (ID: {currentEvent.eventID}, Type: {currentEvent.type})");
            Debug.Log($"Se termine en : Année {eventEndDate.year}, Mois {eventEndDate.month}");
        }
        else
        {
            Debug.Log("Aucun événement en cours actuellement.");
        }
    }
}