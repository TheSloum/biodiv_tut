using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CycleEventManager : MonoBehaviour
{
    [Header("Références")]
    public E_Event eventSystem;
    public J_TimeManager timeManager;
    public E_EventSettings eventSettings;

    // File des événements planifiés pour l'année
    private Queue<ScheduledEvent> eventQueue = new Queue<ScheduledEvent>();

    // Variables pour détecter les changements de date
    private int lastDay, lastMonth, lastYear;

    // Flags pour la gestion des états d'événement
    private bool isEventActive = false;
    private bool waitingCooldown = false;
    private bool coralFestivalDone = false;

    void Start()
    {
        if (timeManager == null)
            timeManager = FindObjectOfType<J_TimeManager>();
        if (eventSystem == null)
            eventSystem = FindObjectOfType<E_Event>();
        if (eventSettings == null)
        {
            Debug.LogError("EventSettings n'est pas assigné.");
            return;
        }

        // Initialiser les variables de temps
        lastDay = timeManager.currentDay;
        lastMonth = timeManager.currentMonth;
        lastYear = timeManager.currentYear;

        FillEventQueue(lastYear);
    }

    void Update()
    {
        int currentDay = timeManager.currentDay;
        int currentMonth = timeManager.currentMonth;
        int currentYear = timeManager.currentYear;

        // Changement d'année : réinitialisation et replanification
        if (currentYear != lastYear)
        {
            coralFestivalDone = false;
            FillEventQueue(currentYear);
            lastYear = currentYear;
        }
        // Changement de mois : déclenchement de la Fête du Corail si c'est le bon mois
        if (currentMonth != lastMonth)
        {
            if (currentMonth == eventSettings.coralFestivalMonth && !coralFestivalDone)
            {
                TriggerCoralFestival();
                coralFestivalDone = true;
            }
            lastMonth = currentMonth;
        }
        // Changement de jour : tenter de démarrer un événement si les conditions sont réunies
        if (currentDay != lastDay)
        {
            if (!isEventActive && !waitingCooldown)
            {
                // On ne déclenche pas d'événement en début/fin de mois selon les réglages
                if (currentDay > eventSettings.noEventStartDays && currentDay < (timeManager.daysPerMonth - eventSettings.noEventEndDays))
                {
                    StartNextEvent();
                }
            }
            lastDay = currentDay;
        }
    }

    // Remplit la file avec 3 événements (1 invasion et 2 normaux) dans un ordre aléatoire prédéfini
    void FillEventQueue(int year)
    {
        eventQueue.Clear();

        List<ScheduledEvent> eventsList = new List<ScheduledEvent>();

        // Sélection d'une invasion aléatoire parmi celles définies
        if (eventSettings.invasionTypes.Count > 0)
        {
            int randomIndex = Random.Range(0, eventSettings.invasionTypes.Count);
            var invasion = eventSettings.invasionTypes[randomIndex];
            eventsList.Add(new ScheduledEvent(EventType.Invasion, invasion.eventID, invasion.name, invasion.durationInMonths));
        }

        // Sélection aléatoire de 2 événements normaux
        if (eventSettings.normalEvents.Count > 0)
        {
            List<NormalEventType> normalPool = new List<NormalEventType>(eventSettings.normalEvents);
            int count = Mathf.Min(2, normalPool.Count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, normalPool.Count);
                var normalEvent = normalPool[randomIndex];
                eventsList.Add(new ScheduledEvent(EventType.Normal, normalEvent.eventID, normalEvent.name, normalEvent.durationInMonths));
                normalPool.RemoveAt(randomIndex);
            }
        }

        // Choix d'un ordre parmi trois possibilités
        int order = Random.Range(1, 4); // 1, 2 ou 3
        Queue<ScheduledEvent> orderedQueue = new Queue<ScheduledEvent>();

        List<ScheduledEvent> normalEvents = eventsList.FindAll(e => e.type == EventType.Normal);
        ScheduledEvent invasionEvent = eventsList.Find(e => e.type == EventType.Invasion);

        if(normalEvents.Count < 2 || invasionEvent == null)
        {
            Debug.LogWarning("Pas assez d'événements pour remplir la file.");
            return;
        }

        switch(order)
        {
            case 1:
                // Normal, Normal, Invasion
                orderedQueue.Enqueue(normalEvents[0]);
                orderedQueue.Enqueue(normalEvents[1]);
                orderedQueue.Enqueue(invasionEvent);
                break;
            case 2:
                // Normal, Invasion, Normal
                orderedQueue.Enqueue(normalEvents[0]);
                orderedQueue.Enqueue(invasionEvent);
                orderedQueue.Enqueue(normalEvents[1]);
                break;
            case 3:
                // Invasion, Normal, Normal
                orderedQueue.Enqueue(invasionEvent);
                orderedQueue.Enqueue(normalEvents[0]);
                orderedQueue.Enqueue(normalEvents[1]);
                break;
        }

        eventQueue = orderedQueue;
    }

    // Démarre l'événement suivant dans la file
    void StartNextEvent()
    {
        if (eventQueue.Count == 0)
        {
            Debug.Log("Aucun événement planifié.");
            return;
        }
        ScheduledEvent nextEvent = eventQueue.Dequeue();
        isEventActive = true;
        Debug.Log("Démarrage de l'événement : " + nextEvent.name);

        // Déclenche l'événement avec son ID et sa durée spécifique (en mois)
        eventSystem.TriggerEvent(nextEvent.eventID, nextEvent.durationInMonths);

        // Gestion spécifique pour une invasion
        if (nextEvent.type == EventType.Invasion)
        {
            var invasionType = eventSettings.invasionTypes.Find(inv => inv.eventID == nextEvent.eventID);
            if (invasionType != null && invasionType.prefabs.Length > 0)
            {
                E_FishSpawner.Instance.EnableInvasionMode(invasionType.prefabs[0]);
                Debug.Log("Mode invasion activé avec le prefab : " + invasionType.prefabs[0].name);
            }
            else
            {
                Debug.LogWarning("Prefab d'invasion non trouvé pour l'event ID " + nextEvent.eventID);
            }
        }
    }

    // Fin d'événement et lancement d'un court cooldown avant le prochain événement
    public void EndEvent()
    {
        isEventActive = false;
        Debug.Log("Événement terminé.");
        waitingCooldown = true;
        // Cooldown de 5 secondes (ajustable)
        Invoke("ResetCooldown", 5f);
    }

    void ResetCooldown()
    {
        waitingCooldown = false;
    }

    // Déclenche directement la Fête du Corail
    void TriggerCoralFestival()
    {
        Debug.Log("Déclenchement de la Fête du Corail.");
        eventSystem.TriggerEvent(3, eventSettings.coralFestivalDuration);
    }

    // Classe interne pour représenter un événement planifié
    private class ScheduledEvent
    {
        public EventType type;
        public int eventID;
        public string name;
        public int durationInMonths;

        public ScheduledEvent(EventType type, int eventID, string name, int durationInMonths)
        {
            this.type = type;
            this.eventID = eventID;
            this.name = name;
            this.durationInMonths = durationInMonths;
        }
    }

    // Types d'événements possibles
    private enum EventType { Normal, Invasion, CoralFestival }
}
