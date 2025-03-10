using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class E_CycleEventManager : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence au script E_Event qui déclenche concrètement les événements")]
    public E_Event eventSystem;

    [Tooltip("Référence au TimeManager pour suivre le temps (jour/mois/année)")]
    public J_TimeManager timeManager;

    [Tooltip("Référence aux paramètres des événements (ScriptableObject)")]
    public E_EventSettings eventSettings;

    // Cooldown pour les événements normaux (clé = eventID, valeur = année minimale de réactivation)
    private Dictionary<int, int> normalEventCooldowns = new Dictionary<int, int>();

    // Pour éviter des invasions consécutives, on stocke le dernier ID d'invasion déclenché
    private int lastInvasionEventID = -1;

    // Historique des invasions sur les 3 dernières années : (année, eventID)
    private List<(int year, int eventID)> invasionHistory = new List<(int, int)>();

    // File d'événements planifiés pour l'année
    private Queue<ScheduledEvent> yearlyEventQueue = new Queue<ScheduledEvent>();

    // Flags et états
    private bool coralFestivalPending = false;
    private bool coralFestivalDoneThisYear = false;
    private bool waitingNoEventCooldown = false;
    private bool isEventActive = false;
    private ScheduledEvent currentEvent = null;
    // Date de fin de l'événement en cours (année, mois)
    private (int year, int month) eventEndDate;

    // Pour éviter de remplir la file plusieurs fois pour une même année
    private int lastFilledQueueYear = -1;

    // Types d'événement
    private enum EventType { Normal, Invasion, CoralFestival }

    // Classe interne pour représenter un événement planifié
    private class ScheduledEvent
    {
        public EventType type;
        public int eventID;     // ID utilisé par E_Event pour .TriggerEvent()
        public string name;     // Nom de l'événement (pour debug)
    }

    private static E_CycleEventManager _instance;
    public static E_CycleEventManager Instance => _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Assure la persistance entre scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

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

        // Abonnement aux événements du TimeManager
        timeManager.OnDayChanged += OnDayChanged;
        timeManager.OnMonthChanged += OnMonthChanged;
        timeManager.OnYearChanged  += OnYearChanged;

        // Remplissage de la file annuelle pour l'année en cours
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

    // Initialisation du système d'événements
    void InitializeEventSystem()
    {
        lastInvasionEventID = -1;
        invasionHistory.Clear();
        normalEventCooldowns.Clear();
        Debug.Log("Système d'événements initialisé.");
    }

    // Appelé chaque fois que le jour change
    void OnDayChanged(int newDay, int newMonth)
    {
        // Vérifie que nous sommes hors des périodes "no event" définies dans les settings
        if (!isEventActive && !waitingNoEventCooldown)
        {
            if (newDay > eventSettings.noEventStartDays && newDay < (timeManager.daysPerMonth - eventSettings.noEventEndDays))
            {
                TryStartCycleEvent();
            }
        }
    }

    // Appelé chaque fois que le mois change
    void OnMonthChanged(int newMonth)
    {
        int currentYear = timeManager.GetCurrentYear();
        // Vérifier la Fête du Corail au mois désigné
        if (newMonth == eventSettings.coralFestivalMonth && !coralFestivalDoneThisYear)
        {
            TriggerCoralFestivalIfNeeded(currentYear);
        }
    }

    // Appelé lors du changement d'année
    void OnYearChanged(int newYear)
    {
        coralFestivalDoneThisYear = false;
        FillYearlyEventQueue(newYear);
        CleanupNormalEventCooldowns(newYear);
        CleanupInvasionHistory(newYear);
    }

    // Remplit la file annuelle d'événements (par exemple : 1 invasion, 2 événements normaux, et la Fête du Corail tous les 2 ans)
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

        // Planification des invasions
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

        // Planification des événements normaux
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

        // Planification de la Fête du Corail (tous les 2 ans)
        if (year % 2 == 0)
        {
            ScheduledEvent sched = new ScheduledEvent { type = EventType.CoralFestival, eventID = 3, name = "Fête du Corail" };
            yearlyEventQueue.Enqueue(sched);
            Debug.Log("Enfilage de la Fête du Corail.");
        }

        Debug.Log($"File annuelle complétée pour l'année {year} avec {yearlyEventQueue.Count} événements.");
    }

    // Tente de déclencher un événement au début d'un cycle
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

        int eventDurationMonths = 0;
        if (next.type == EventType.CoralFestival)
        {
            eventDurationMonths = eventSettings.coralFestivalDuration; // Durée spécifique pour la Fête du Corail
        }
        else
        {
            // Pour une invasion ou un événement normal, la durée est choisie aléatoirement entre 2 et 4 mois
            eventDurationMonths = Random.Range(2, 5); // 2, 3 ou 4 mois
        }

        int startMonth = timeManager.GetCurrentMonth();
        int startYear = timeManager.GetCurrentYear();
        int endMonth = startMonth + eventDurationMonths;
        int endYear = startYear;
        while (endMonth > 12)
        {
            endMonth -= 12;
            endYear++;
        }
        eventEndDate = (endYear, endMonth);
        Debug.Log($"Date de fin de l'événement : Année {endYear}, Mois {endMonth}");

        StartCoroutine(StartEvent(next.eventID, eventDurationMonths, next.type));
    }

    // Coroutine qui déclenche l'événement, attend sa durée, puis applique le cooldown global de 2 minutes
    IEnumerator StartEvent(int eventID, int durationMonths, EventType type)
    {
        isEventActive = true;
        eventSystem.TriggerEvent(eventID);
        Debug.Log($"Événement déclenché : {GetEventNameByID(eventID)}, durée {durationMonths} mois");

        // Pour une invasion, on déclenche l'instanciation des préfabriqués et on met à jour l'historique
        if (type == EventType.Invasion)
        {
            TriggerInvasion(eventID);
            int currentYear = timeManager.GetCurrentYear();
            invasionHistory.Add((currentYear, eventID));
            lastInvasionEventID = eventID;
        }

        // Conversion de la durée en secondes réelles (exemple : 1 mois = 300 secondes)
        float realDuration = durationMonths * 300f;
        if (type == EventType.CoralFestival)
        {
            realDuration = eventSettings.coralFestivalDuration * 300f;
        }
        yield return new WaitForSeconds(realDuration);

        isEventActive = false;
        currentEvent = null;
        Debug.Log($"Événement terminé : {GetEventNameByID(eventID)}");

        // Cooldown global : aucune nouvel événement pendant 2 minutes (correspondant à 12 jours en jeu)
        waitingNoEventCooldown = true;
        yield return new WaitForSeconds(120f);
        waitingNoEventCooldown = false;
    }

    // Déclenche la Fête du Corail si les conditions sont remplies
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

    // Détermine si le joueur est en mode exploration (basé ici sur le nom de la scène active)
    bool IsPlayerExploring()
    {
        return SceneManager.GetActiveScene().name == "Exploration_main";
    }

    // Sélectionne le prochain ID d'invasion en évitant de répéter immédiatement le même
    // et en s'assurant que sur 3 ans, tous les types d'invasion se produisent au moins une fois.
    int GetNextInvasionID(int currentYear)
    {
        // Récupère la liste de tous les IDs d'invasion
        List<int> allInvasions = eventSettings.invasionTypes.Select(inv => inv.eventID).ToList();
        // Filtre ceux qui n'ont pas été utilisés durant les 3 dernières années
        List<int> required = new List<int>();
        foreach (int invID in allInvasions)
        {
            bool found = invasionHistory.Any(entry => entry.eventID == invID && entry.year > currentYear - 3);
            if (!found)
                required.Add(invID);
        }

        List<int> candidatePool = new List<int>();
        if (required.Count > 0)
        {
            candidatePool = required;
        }
        else
        {
            // Sinon, on prend tous les types d'invasion en excluant celui utilisé précédemment
            candidatePool = allInvasions.Where(id => id != lastInvasionEventID).ToList();
        }

        if (candidatePool.Count == 0)
            return -1;

        int chosen = candidatePool[Random.Range(0, candidatePool.Count)];
        return chosen;
    }

    // Sélectionne le prochain événement normal en vérifiant le cooldown (5 ans)
    int GetNextNormalEventID(int currentYear)
    {
        List<int> available = eventSettings.normalEvents
            .Select(e => e.eventID)
            .Where(id => !normalEventCooldowns.ContainsKey(id) || normalEventCooldowns[id] <= currentYear)
            .ToList();

        if (available.Count == 0)
            return -1;

        int chosen = available[Random.Range(0, available.Count)];
        // Applique un cooldown de 5 ans
        normalEventCooldowns[chosen] = currentYear + 5;
        return chosen;
    }

    // Nettoie les cooldowns des événements normaux expirés
    void CleanupNormalEventCooldowns(int currentYear)
    {
        var keysToRemove = normalEventCooldowns.Where(kvp => kvp.Value <= currentYear).Select(kvp => kvp.Key).ToList();
        foreach (int key in keysToRemove)
        {
            normalEventCooldowns.Remove(key);
            Debug.Log($"Cooldown terminé pour l'événement normal ID {key}");
        }
    }

    // Nettoie l'historique des invasions pour ne conserver que les 3 dernières années
    void CleanupInvasionHistory(int currentYear)
    {
        invasionHistory = invasionHistory.Where(entry => entry.year > currentYear - 3).ToList();
    }

    // Pour les invasions, instancie les préfabriqués correspondants
    void TriggerInvasion(int invasionID)
    {
        var invasion = eventSettings.invasionTypes.FirstOrDefault(inv => inv.eventID == invasionID);
        if (invasion != null && invasion.prefabs.Length > 0)
        {
            foreach (var prefab in invasion.prefabs)
            {
                Instantiate(prefab, Vector3.zero, Quaternion.identity);
            }
            Debug.Log($"Invasion déclenchée : {invasion.name} (ID: {invasionID})");
        }
        else
        {
            Debug.LogWarning($"Aucune prefab trouvée pour l'invasion ID {invasionID}.");
        }
    }

    // Récupère le nom d'un événement en fonction de son ID
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
}
