using System;
using UnityEngine;

public static class EventBus
{
    public static event Action OnCoralFestivalStart;
    public static event Action OnCoralFestivalEnd;

    public static void CoralFestivalStart()
    {
        OnCoralFestivalStart?.Invoke();
        Debug.Log("EventBus : Coral Festival Start event triggered.");
    }

    public static void CoralFestivalEnd()
    {
        OnCoralFestivalEnd?.Invoke();
        Debug.Log("EventBus : Coral Festival End event triggered.");
    }
}
