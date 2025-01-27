// E_EventSettings.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EventSettings", menuName = "Events/New Event Settings")]
public class E_EventSettings : ScriptableObject
{
    [Header("General Settings")]
    public float defaultFishSpawnRate = 3f;
    public float eventTransitionDuration = 1f;

    [Header("Invasion Settings")]
    public GameObject[] jellyfishPrefabs;
    public GameObject[] lionfishPrefabs;
    public GameObject[] barracudaPrefabs;

    [Header("Festival Settings")] 
    public float festivalSpawnMultiplier = 1.5f;
    public Color festivalTextColor = Color.yellow;

    [Header("Trash Wave Settings")]
    public float trashWaveFishReduction = 0.5f;
    public float trashWaveSpawnIncrease = 2f;
    public Color trashTextColor = Color.gray;

    [Header("Overlay Settings")]
    public float overlayMaxOpacity = 0.7f;
    public float overlayFadeOutDuration = 2f;

    [Header("Default Settings")] 
    public GameObject[] defaultFishPrefabs;
}