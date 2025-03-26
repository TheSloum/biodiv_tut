using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CycleEventManager : MonoBehaviour
{
    [Header("Références")]
    public E_Event eventSystem;
    public J_TimeManager timeManager;
    public E_EventSettings eventSettings;

    // Queue
    private Queue<ScheduledEvent> eventQueue = new Queue<ScheduledEvent>();

    // Détecter changement de date
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
            Debug.LogError("EventSettings non assigné.");
            return;
        }

        // Variables de temps
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
        
        // Changement de jour : vérifier si un événement doit être déclenché aujourd'hui
        if (currentDay != lastDay)
        {
            if (!isEventActive && !waitingCooldown && eventQueue.Count > 0)
            {
                // Vérifier si la date actuelle correspond à celle du prochain événement planifié
                ScheduledEvent nextEvent = eventQueue.Peek();
                if (currentMonth == nextEvent.scheduledMonth && currentDay == nextEvent.scheduledDay)
                {
                    eventQueue.Dequeue();
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
                            Debug.Log("Mode invasion pour le poisson : " + invasionType.prefabs[0].name);
                        }
                        else
                        {
                            Debug.LogWarning("Prefab d'invasion non trouvé pour l'event ID " + nextEvent.eventID);
                        }
                    }
                }
            }
            lastDay = currentDay;
        }
    }

    // Remplit la file avec 3 événements (1 invasion et 2 normaux) dans un ordre aléatoire prédéfini.
    // L'événement Fête du Corail (id 3) est exclu de cette file.
    void FillEventQueue(int year)
    {
        eventQueue.Clear();

        List<ScheduledEvent> eventsList = new List<ScheduledEvent>();

        // Sélection d'une invasion aléatoire parmi celles définies
        if (eventSettings.invasionTypes.Count > 0)
        {
            int randomIndex = Random.Range(0, eventSettings.invasionTypes.Count);
            var invasion = eventSettings.invasionTypes[randomIndex];
            // Par précaution, on exclut aussi l'id 3 si jamais il est présent dans invasionTypes
            if(invasion.eventID != 3)
            {
                eventsList.Add(new ScheduledEvent(EventType.Invasion, invasion.eventID, invasion.name, invasion.durationInMonths));
            }
        }

        // Sélection aléatoire de 2 événements normaux en excluant la Fête du Corail (id 3)
        if (eventSettings.normalEvents.Count > 0)
        {
            List<NormalEventType> normalPool = new List<NormalEventType>();
            foreach (var normal in eventSettings.normalEvents)
            {
                if (normal.eventID != 3)
                {
                    normalPool.Add(normal);
                }
            }
            
            int count = Mathf.Min(2, normalPool.Count);
            for (int i = 0; i < count; i++)
            {
                int randomIndex = Random.Range(0, normalPool.Count);
                var normalEvent = normalPool[randomIndex];
                eventsList.Add(new ScheduledEvent(EventType.Normal, normalEvent.eventID, normalEvent.name, normalEvent.durationInMonths));
                normalPool.RemoveAt(randomIndex);
            }
        }

        if (eventsList.Count < 3)
        {
            Debug.LogWarning("Pas assez d'événements pour remplir la file.");
            return;
        }

        // On récupère les événements par type pour l'ordonnancement
        List<ScheduledEvent> normalEvents = eventsList.FindAll(e => e.type == EventType.Normal);
        ScheduledEvent invasionEvent = eventsList.Find(e => e.type == EventType.Invasion);
        if(normalEvents.Count < 2 || invasionEvent == null)
        {
            Debug.LogWarning("Pas assez d'événements pour remplir la file.");
            return;
        }

        // Application d'un ordre aléatoire pour déterminer la séquence des événements
        Queue<ScheduledEvent> orderedQueue = new Queue<ScheduledEvent>();
        int order = Random.Range(1, 4); // 1, 2 ou 3

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

        // Génération de trois dates chronologiques en respectant un intervalle d'au moins 1 mois entre chaque événement.
        int firstEventMonth = Random.Range(1, 9); // 1 inclus, 9 exclus => max 8
        int secondEventMonth = Random.Range(firstEventMonth + 2, 11); 
        int thirdEventMonth = Random.Range(secondEventMonth + 2, 13);
        // Pour le jour, on choisit aléatoirement une valeur comprise entre noEventStartDays+1 et (daysPerMonth - noEventEndDays)
        int firstEventDay = Random.Range(eventSettings.noEventStartDays + 1, timeManager.daysPerMonth - eventSettings.noEventEndDays);
        int secondEventDay = Random.Range(eventSettings.noEventStartDays + 1, timeManager.daysPerMonth - eventSettings.noEventEndDays);
        int thirdEventDay = Random.Range(eventSettings.noEventStartDays + 1, timeManager.daysPerMonth - eventSettings.noEventEndDays);

        // On transforme la queue en liste pour pouvoir assigner les dates dans l'ordre généré
        List<ScheduledEvent> orderedEvents = new List<ScheduledEvent>(orderedQueue);
        if(orderedEvents.Count >= 3)
        {
            orderedEvents[0].scheduledMonth = firstEventMonth;
            orderedEvents[0].scheduledDay = firstEventDay;
            orderedEvents[1].scheduledMonth = secondEventMonth;
            orderedEvents[1].scheduledDay = secondEventDay;
            orderedEvents[2].scheduledMonth = thirdEventMonth;
            orderedEvents[2].scheduledDay = thirdEventDay;
        }

        // On vide la queue et on la remplit avec la liste dans l'ordre défini (les dates seront donc chronologiques)
        eventQueue.Clear();
        foreach (var scheduledEvent in orderedEvents)
        {
            eventQueue.Enqueue(scheduledEvent);
            Debug.Log($"Événement '{scheduledEvent.name}' planifié le mois {scheduledEvent.scheduledMonth} jour {scheduledEvent.scheduledDay}");
        }
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

        public int scheduledMonth;
        public int scheduledDay;

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
