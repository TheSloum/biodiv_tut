using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

/// <summary>
/// À brancher avec J_TimeManager et E_Event.
/// </summary>
public class E_CycleEventManager : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Référence au script E_Event (système qui déclenche concrètement les events)")]
    public E_Event eventSystem;

    [Tooltip("Référence au TimeManager pour suivre mois/années")]
    public J_TimeManager timeManager;

    [Header("Paramètres de cycle")]
    [Tooltip("Durée d'un cycle en mois (4 par défaut => 12 mois = 3 cycles)")]
    public int monthsPerCycle = 4;
    
    [Tooltip("Début de la plage 'no-event' en jours (18j = 3mn, si 1 jour = 10s)")]
    public int noEventStartDays = 18; 
    
    [Tooltip("Fin de la plage 'no-event' sur les derniers jours du cycle (idem 18j)")]
    public int noEventEndDays = 18;

    [Tooltip("Délai (en mois) minimum/maximum qu'un event peut durer (sauf Corail)")]
    public Vector2 eventDurationRange = new Vector2(2, 4);

    [Header("Fête du Corail")]
    [Tooltip("Le mois de déclenchement de la fête du corail (6 par défaut)")]
    public int coralFestivalMonth = 6;
    [Tooltip("Durée en mois de la fête du corail (1 mois par défaut)")]
    public int coralFestivalDuration = 1;
    [Tooltip("Si la fête tombe en plein exploration, on la décale à la fin de l'explo")]
    public bool waitForExplorationIfCoralFestival = true;

    [Header("Règles Annuelles")]
    [Tooltip("Nombre minimum d'invasions par an (1 par défaut)")]
    public int invasionsPerYear = 1;
    [Tooltip("Nombre minimum d'events normaux par an (2 par défaut)")]
    public int normalEventsPerYear = 2;
    [Tooltip("Les invasions doivent être uniques dans les 3 ans, etc.")]
    // ... paramétrage supplémentaire

    // ===================================================================================================
    // INTERNES
    // ===================================================================================================
    
    // Représente nos types d’event
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

    // Queue globale d’événements annuels => par ex. [Normal, Normal, Invasion], [Normal, Invasion, Normal], ...
    private Queue<ScheduledEvent> yearlyEventQueue = new Queue<ScheduledEvent>();

    // Gère la liste d’events normaux (avec cooldown 5 ans)
    // Tu peux y mettre tous tes ID (genre 4 = trash wave, 5= algues toxiques, etc.)
    private List<int> allNormalEventIDs = new List<int> { 4 /* VagueDechets */, /*6 = Tempête? 7=AlguesToxiques?*/ };
    private Dictionary<int, int> normalEventNextAvailableYear = new Dictionary<int, int>(); // eventID -> (année min)
    
    // Gère les invasions (3 types : 0=Méduses, 1=PoissonLion, 2=Barracuda)
    // Sur 3 ans, on doit toutes les faire au moins une fois
    private List<int> allInvasionIDs = new List<int> { 0, 1, 2 };
    private Dictionary<int, int> invasionLastYearUsed = new Dictionary<int, int>(); // pour savoir quand on les a utilisées
    
    // Pour ne pas répéter la même invasion deux fois de suite
    private int lastInvasionID = -1;

    // Suivi du cycle actuel
    private int currentCycleIndex = 0;  // ex: 0 => mois 1-4, 1 => mois 5-8, 2 => mois 9-12, etc.

    // Surveiller si un event est en cours
    private bool isEventActive = false;
    // Mémoriser le type de l’event en cours (pour "if (currentEvent == 'INVASION')" etc.)
    private EventType currentEventType = EventType.Normal;

    // Stocke quand l'event actuel se termine (en mois + année)
    private (int year, int month) eventEndDate;

    // Indique si la fête du corail a déjà été déclenchée cette année
    private bool coralFestivalDoneThisYear = false;

    // Pour traquer la "période sans event" suite à un event (12 jours) => 2 minutes
    private bool waitingNoEventCooldown = false;

    // Pour éviter de remplir la queue plus d'une fois par an
    private int lastFilledQueueYear = -1;

    // ===================================================================================================
    // UNITY
    // ===================================================================================================
    private void Start()
    {
        if (!timeManager) timeManager = FindObjectOfType<J_TimeManager>();
        if (!eventSystem) eventSystem = FindObjectOfType<E_Event>();

        // On s’abonne aux events du timeManager
        timeManager.OnMonthChanged += OnMonthChanged;
        timeManager.OnYearChanged  += OnYearChanged;

        // Initialisation
        FillYearlyEventQueue(timeManager.GetCurrentYear());
    }

    private void OnDestroy()
    {
        if (timeManager)
        {
            timeManager.OnMonthChanged -= OnMonthChanged;
            timeManager.OnYearChanged  -= OnYearChanged;
        }
    }

    // ===================================================================================================
    // EVENTS TIME MANAGER
    // ===================================================================================================
    private void OnMonthChanged(int newMonth)
    {
        // Calcul de l'index du cycle actuel (ex : mois 1-4 = cycle 0, mois 5-8 = cycle 1, ...)
        int currentYear = timeManager.GetCurrentYear();
        currentCycleIndex = (newMonth - 1) / monthsPerCycle;  
        
        // Check Fête du Corail
        if (newMonth == coralFestivalMonth && !coralFestivalDoneThisYear)
        {
            TriggerCoralFestivalIfNeeded(currentYear);
        }

        // Si un event est planifié pour ce cycle et on est dans la « fenêtre »  
        // (pas dans les 18 premiers jours, pas dans les 18 derniers jours, pas d’autre event en cours),
        // alors on déclenche le début de cycle d’event.
        // => Convertis le jour actuel en un "jour du mois"
        int currentDay = timeManager.currentDay;

        if (!isEventActive && !waitingNoEventCooldown)
        {
            // Vérif si on est hors noEventStartDays ET hors noEventEndDays
            if (currentDay > noEventStartDays && currentDay < (timeManager.daysPerMonth - noEventEndDays))
            {
                // On tente de déclencher un event si un cycle "doit" en avoir un
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
        UpdateNormalEventCooldowns(newYear);
        
        // Vérifier la contrainte "dans 3 ans, tous les invasions doivent passer"
        // (Tu peux coder la validation ici. Par exemple, si on voit qu’une invasion n’a pas eu lieu
        // dans un intervalle de 3 ans, on la force dans la queue à la place d’une autre.)
    }

    // ===================================================================================================
    // FONCTIONS PRINCIPALES LOGIQUES
    // ===================================================================================================

    /// <summary>
    /// Remplit la queue d’events annuels (1 an = 3 cycles).
    /// On doit avoir 1 invasion, 2 normal, etc.  
    /// On évite 2 invasions d’affilée d’une année sur l’autre.
    /// Basé sur ton pseudo-code.
    /// </summary>
    private void FillYearlyEventQueue(int year)
    {
        if (lastFilledQueueYear == year) return; // évite de remplir deux fois la même année

        lastFilledQueueYear = year;
        
        // Regarde si le dernier event (ou currentEvent) était une invasion ?
        // Dans ce script on gère l'info `currentEventType`.
        bool lastWasInvasion = (currentEventType == EventType.Invasion);

        int rng;
        if (lastWasInvasion)
            rng = Random.Range(1, 3); // 1..2
        else
            rng = Random.Range(1, 4); // 1..3 (en C# le max est exclusif, donc Range(1,4) = 1,2,3)

        // Comme dans le pseudo-code
        // if rng=1 => [NORMAL, NORMAL, INVASION]
        // if rng=2 => [NORMAL, INVASION, NORMAL]
        // else      => [INVASION, NORMAL, NORMAL]
        
        // On crée ces 3 events programmés pour l’année en cours
        List<EventType> pattern = new List<EventType>();
        if (rng == 1) {
            pattern.Add(EventType.Normal);
            pattern.Add(EventType.Normal);
            pattern.Add(EventType.Invasion);
        }
        else if (rng == 2) {
            pattern.Add(EventType.Normal);
            pattern.Add(EventType.Invasion);
            pattern.Add(EventType.Normal);
        }
        else {
            pattern.Add(EventType.Invasion);
            pattern.Add(EventType.Normal);
            pattern.Add(EventType.Normal);
        }

        // On convertit en ScheduledEvent (en choisissant un ID concret pour l’invasion ou le normal)
        foreach (EventType t in pattern)
        {
            var sched = new ScheduledEvent { type = t };
            switch (t)
            {
                case EventType.Normal:
                    sched.eventID = PickNormalEventID(year);
                    sched.name = "Normal#" + sched.eventID;
                    break;
                case EventType.Invasion:
                    sched.eventID = PickInvasionID(year);
                    sched.name = "Invasion#" + sched.eventID;
                    break;
                case EventType.CoralFestival:
                    sched.eventID = 3; // dans ton E_Event, 3 = Fête du Corail
                    sched.name = "Fête du Corail";
                    break;
            }
            yearlyEventQueue.Enqueue(sched);
        }
    }

    /// <summary>
    /// Lorsqu’un nouveau cycle commence, on pioche dans la queue d’events annuels.
    /// Puis on détermine la durée de l’event.
    /// Puis on planifie le déclenchement effectif (si on est hors zone "no-event", etc.).
    /// </summary>
    private void TryStartCycleEvent()
    {
        // On vérifie d’abord si la queue n’est pas vide
        if (yearlyEventQueue.Count == 0) return;

        // On pop un event
        ScheduledEvent next = yearlyEventQueue.Dequeue();
        currentEventType = next.type;

        // Calcul d’une durée (en mois) si ce n’est pas la fête du corail
        int eventDurationMonths = 0;
        if (next.type == EventType.CoralFestival)
        {
            eventDurationMonths = coralFestivalDuration;
        }
        else
        {
            eventDurationMonths = Random.Range((int)eventDurationRange.x, (int)eventDurationRange.y + 1);
        }

        // On calcule la date de fin (mois + année)
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

        // On déclenche l’event
        StartCoroutine(StartEvent(next.eventID, eventDurationMonths));
    }

    /// <summary>
    /// Coroutine qui gère l’event en cours : déclenche, attend la fin, applique la règle
    /// "2 minutes/12 jours de temps ou il ne peut pas avoir d'event" après la fin, etc.
    /// </summary>
    private IEnumerator StartEvent(int eventID, int eventDurationMonths)
    {
        isEventActive = true;
        // On déclenche "réellement" via E_Event
        eventSystem.TriggerEvent(eventID);

        // On attend en temps "réel" l’équivalent de la durée en mois => 
        // Dans ton jeu : 1 mois = 5 minutes => 1 mois = 300 secondes
        // eventDurationMonths * 300 => sauf pour la Fête du Corail, c’est plus "symbolique"
        
        // Pour être cohérent : 2..4 mois => 2..4 * 300s => 600..1200s
        // Mais comme ton TimeManager simule le temps, tu peux soit faire un WaitUntil condition 
        // sur la date in-game, soit faire un WaitForSeconds. À toi de choisir.
        float realDuration = eventDurationMonths * 300f;
        if (currentEventType == EventType.CoralFestival)
        {
            // Parfois on veut la faire durer un "mois" complet => 300s
            realDuration = coralFestivalDuration * 300f;
        }

        yield return new WaitForSeconds(realDuration);

        // L’event est terminé
        isEventActive = false;

        // On applique le cooldown global : 2 minutes / 12 jours => 120s
        // => plus aucune event possible pendant ce laps.
        waitingNoEventCooldown = true;
        yield return new WaitForSeconds(120f);
        waitingNoEventCooldown = false;
    }

    /// <summary>
    /// Déclenche la fête du corail si on est au mois = coralFestivalMonth et si c’est tous les 2 ans
    /// (d’après tes règles, "au milieu du 2e cycle tous les 2 ans"). 
    /// Ici on simplifie un peu la condition de la "2e année" => year%2==0
    /// </summary>
    private void TriggerCoralFestivalIfNeeded(int currentYear)
    {
        // Règle "au milieu du 2e cycle tous les 2 ans" => on suppose year%2==0
        if (currentYear % 2 == 0)
        {
            if (waitForExplorationIfCoralFestival && IsPlayerExploring())
            {
                // On attend la fin de l'exploration, puis on déclenche
                StartCoroutine(WaitEndExplorationAndTriggerCoral());
            }
            else
            {
                // On force directement l’event "3" = Fête du corail
                eventSystem.TriggerEvent(3);
            }

            coralFestivalDoneThisYear = true;
        }
    }

    private IEnumerator WaitEndExplorationAndTriggerCoral()
    {
        // On attend que le joueur ne soit plus en exploration
        yield return new WaitUntil(() => !IsPlayerExploring());
        // On déclenche la fête
        eventSystem.TriggerEvent(3);
        coralFestivalDoneThisYear = true;
    }

    /// <summary>
    /// Vérifie si le joueur est en exploration. Dans ton code final, 
    /// remplace par la bonne logique (manager d’explo, etc.)
    /// </summary>
    private bool IsPlayerExploring()
    {
        // TODO: coder la vraie condition
        return false;
    }

    // ===================================================================================================
    // PICKERS D'ID (pour normal / invasion) + COOLDOWNS
    // ===================================================================================================

    /// <summary>
    /// Choisit un ID d'invasion (0=Méduses, 1=PoissonLion, 2=Barracuda),  
    /// évite de prendre deux fois d’affilée la même,  
    /// et assure que sur 3 ans, on aura toutes les invasions au moins une fois (à toi de coder la logique).
    /// </summary>
    private int PickInvasionID(int currentYear)
    {
        // Filtre la liste d’invasions possible
        var possible = allInvasionIDs.Where(id => id != lastInvasionID).ToList();

        // On prend aléatoirement
        int chosen = possible[Random.Range(0, possible.Count)];
        
        // Stock
        lastInvasionID = chosen;
        invasionLastYearUsed[chosen] = currentYear;
        
        return chosen;
    }

    /// <summary>
    /// Choisit un ID d’event normal parmi ceux disponibles,
    /// en respectant la règle : "Une fois qu'un event normal est arrivé, 
    /// il doit attendre min 5 ans avant de pouvoir revenir".
    /// </summary>
    private int PickNormalEventID(int currentYear)
    {
        // Filtre ceux dont la "nextAvailableYear" est <= currentYear
        var freeEvents = allNormalEventIDs
            .Where(id => !normalEventNextAvailableYear.ContainsKey(id) || normalEventNextAvailableYear[id] <= currentYear)
            .ToList();

        if (freeEvents.Count == 0)
        {
            // Si on n’en a pas, on force un par défaut (ou on ignore, à toi de voir)
            return 4; // ex. "VagueDechets"
        }

        int chosen = freeEvents[Random.Range(0, freeEvents.Count)];

        // On set le prochain retour possible
        normalEventNextAvailableYear[chosen] = currentYear + 5;

        return chosen;
    }

    /// <summary>
    /// Chaque début d’année, on peut « libérer » certains events dont le cooldown arrive à échéance.
    /// </summary>
    private void UpdateNormalEventCooldowns(int newYear)
    {
        // Parcours des events, si la nextAvailableYear <= newYear, c’est “revenu”.
        // Mais comme on a déjà géré la condition dans le pick, on peut simplement nettoyer le dico :
        List<int> toRemove = new List<int>();
        foreach (var kvp in normalEventNextAvailableYear)
        {
            if (kvp.Value <= newYear)
                toRemove.Add(kvp.Key);
        }

        foreach (var id in toRemove)
        {
            normalEventNextAvailableYear.Remove(id);
        }
    }
}
