using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_CycleEventManager : MonoBehaviour
{
    public E_Event eventSystem;
    public J_TimeManager timeManager;
    public E_EventSettings eventSettings;

    private Queue<ScheduledEvent> eventQueue = new Queue<ScheduledEvent>();
    private int lastDay, lastMonth, lastYear;
    private bool isEventActive = false;
    private bool waitingCooldown = false;

    private enum EventType { Normal, Invasion, CoralFestival }

    void Start()
    {
        if (timeManager == null) timeManager = FindObjectOfType<J_TimeManager>();
        if (eventSystem == null) eventSystem = FindObjectOfType<E_Event>();

        lastDay = timeManager.currentDay;
        lastMonth = timeManager.currentMonth;
        lastYear = timeManager.currentYear;

        FillEventQueue(lastYear);
    }

    void Update()
    {
        if (timeManager == null) return;

        int currentDay = timeManager.currentDay;
        int currentMonth = timeManager.currentMonth;
        int currentYear = timeManager.currentYear;

        if (currentYear != lastYear)
        {
            FillEventQueue(currentYear);
            lastYear = currentYear;
        }

        if (currentDay != lastDay)
        {
            if (!isEventActive && !waitingCooldown && eventQueue.Count > 0)
            {
                ScheduledEvent nextEvent = eventQueue.Peek();
                if (currentMonth == nextEvent.scheduledMonth && currentDay == nextEvent.scheduledDay)
                {
                    eventQueue.Dequeue();
                    ExecuteScheduledEvent(nextEvent);
                }
            }
            lastDay = currentDay;
        }
        lastMonth = currentMonth;
    }

    void FillEventQueue(int year)
    {
        eventQueue.Clear();
        List<ScheduledEvent> eventsList = new List<ScheduledEvent>();

        if (year % eventSettings.coralFestivalCycle == 0)
        {
            ScheduledEvent coral = new ScheduledEvent(EventType.CoralFestival, 3, "Fete du Corail", eventSettings.coralFestivalDuration);
            coral.scheduledMonth = eventSettings.coralFestivalMonth;
            coral.scheduledDay = 15;
            eventsList.Add(coral);
        }

        if (eventSettings.invasionTypes.Count > 0)
        {
            InvasionType inv = eventSettings.invasionTypes[Random.Range(0, eventSettings.invasionTypes.Count)];
            eventsList.Add(new ScheduledEvent(EventType.Invasion, inv.eventID, inv.name, 1));
        }

        for (int i = 0; i < 2; i++)
        {
            if (eventSettings.normalEvents.Count > 0)
            {
                NormalEventType norm = eventSettings.normalEvents[Random.Range(0, eventSettings.normalEvents.Count)];
                eventsList.Add(new ScheduledEvent(EventType.Normal, norm.eventID, norm.name, 1));
            }
        }

        int lastMonthUsed = 0;
        foreach (var ev in eventsList)
        {
            if (ev.type != EventType.CoralFestival)
            {
                int rMonth;
                do
                {
                    rMonth = Random.Range(1, 13);
                } while (rMonth == eventSettings.coralFestivalMonth || Mathf.Abs(rMonth - lastMonthUsed) < 2);

                ev.scheduledMonth = rMonth;
                ev.scheduledDay = Random.Range(eventSettings.noEventStartDays + 1, timeManager.daysPerMonth - eventSettings.noEventEndDays);
                lastMonthUsed = rMonth;
            }
        }

        eventsList.Sort((a, b) => a.scheduledMonth.CompareTo(b.scheduledMonth));
        foreach (var ev in eventsList) eventQueue.Enqueue(ev);
    }

    void ExecuteScheduledEvent(ScheduledEvent ev)
    {
        isEventActive = true;
        eventSystem.TriggerEvent(ev.eventID, ev.durationInMonths);

        if (ev.type == EventType.Invasion)
        {
            var invData = eventSettings.invasionTypes.Find(i => i.eventID == ev.eventID);
            if (invData != null && invData.prefabs.Length > 0)
                E_FishSpawner.Instance.EnableInvasionMode(invData.prefabs[0]);
        }
    }

    public void EndEvent()
    {
        isEventActive = false;
        waitingCooldown = true;
        Invoke("ResetCooldown", 5f);
    }

    void ResetCooldown() => waitingCooldown = false;

    private class ScheduledEvent
    {
        public EventType type;
        public int eventID;
        public string name;
        public int durationInMonths;
        public int scheduledMonth;
        public int scheduledDay;

        public ScheduledEvent(EventType t, int id, string n, int d)
        {
            type = t; eventID = id; name = n; durationInMonths = d;
        }
    }
}