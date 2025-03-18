using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EventSettings", menuName = "Events/New Event Settings")]
public class E_EventSettings : ScriptableObject
{
    [Header("General Settings")]
    public float defaultFishSpawnRate = 3f;
    public float eventTransitionDuration = 1f;

    [Header("Cycle Settings")]
    public int monthsPerCycle = 4;
    public int noEventStartDays = 5;
    public int noEventEndDays = 5;
    public Vector2 eventDurationRange = new Vector2(1, 3);
    public int invasionsPerYear = 1;
    public int normalEventsPerYear = 2;

    [Header("Coral Festival Settings")]
    public int coralFestivalMonth = 6;
    public int coralFestivalDuration = 1;
    public bool waitForExplorationIfCoralFestival = true;

    [Header("Invasion Settings")]
    public List<InvasionType> invasionTypes = new List<InvasionType>();

    [Header("Normal Events")]
    public List<NormalEventType> normalEvents = new List<NormalEventType>();

    [Header("Cooldown & Festival Settings")]
    public int invasionCooldownYears = 3; // Exemple : 3 ans de cooldown pour les invasions
    public int normalEventCooldownYears = 5; // Exemple : 5 ans de cooldown pour les événements normaux

    [Header("Festival and Trash Wave Settings")]
    public float festivalSpawnMultiplier = 1.5f;
    public Color festivalTextColor = Color.yellow;
    public float trashWaveFishReduction = 0.5f;
    public float trashWaveSpawnIncrease = 2f;
    public Color trashTextColor = Color.gray;

    [Header("Overlay Settings")]
    public float overlayMaxOpacity = 0.7f;
    public float overlayFadeOutDuration = 2f;

    [Header("Default Settings")]
    public GameObject[] defaultFishPrefabs;
}

[System.Serializable]
public class InvasionType
{
    public string name;
    public int eventID;
    public GameObject[] prefabs;
    // Durée spécifique (en mois) pour cet événement d'invasion
    public int durationInMonths = 2; 
}

[System.Serializable]
public class NormalEventType
{
    public string name;
    public int eventID;
    // Durée spécifique (en mois) pour cet événement normal
    public int durationInMonths = 2; 
}
