using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class E_Event : MonoBehaviour
{
    // Références externes
    public SpriteRenderer blackOverlayRenderer;
    public TextMeshProUGUI eventText;
    public E_EventSettings settings;

    [Header("Timing")]
    public float eventDuration = 60f;
    public float textFadeInDuration = 1f;
    public float textVisibleDuration = 2f;
    public float textFadeOutDuration = 1f;
    public float overlayFadeInDuration = 3f;

    [Header("Debug")]
    public int debugEventNumber = -1;

    // Références internes
    private Dictionary<int, System.Action> eventDictionary;
    private E_FishSpawner fishSpawner;
    private E_TrashSpawner trashSpawner;
    private GameObject[] originalFishPrefabs;
    private float originalFishSpawnRate;
    private float originalTrashSpawnRate;
    private bool isEventActive = false;

    void Start()
    {
        InitializeReferences();
        SetupEventDictionary();
        StoreOriginalValues();
    }

    void Update()
    {
        // Déclenchement manuel avec la touche E
        if(Input.GetKeyDown(KeyCode.E) && debugEventNumber >= 0 && !isEventActive)
        {
            TriggerEvent(debugEventNumber);
        }
    }

    void InitializeReferences()
    {
        fishSpawner = FindObjectOfType<E_FishSpawner>();
        trashSpawner = FindObjectOfType<E_TrashSpawner>();
        
        // Cache le texte au départ
        if(eventText != null)
        {
            eventText.color = new Color(eventText.color.r, eventText.color.g, eventText.color.b, 0f);
            eventText.gameObject.SetActive(false);
        }
    }

    void SetupEventDictionary()
    {
        // Mappage ID -> Événement
        eventDictionary = new Dictionary<int, System.Action>
        {
            {0, () => StartCoroutine(TriggerJellyfishInvasion())},
            {1, () => StartCoroutine(TriggerLionfishInvasion())},
            {2, () => StartCoroutine(TriggerBarracudaInvasion())},
            {3, () => StartCoroutine(TriggerCoralFestival())},
            {4, () => StartCoroutine(TriggerTrashWave())}
        };
    }

    void StoreOriginalValues()
    {
        // Sauvegarde des valeurs initiales
        if(fishSpawner != null)
        {
            originalFishPrefabs = fishSpawner.fishPrefabs;
            originalFishSpawnRate = fishSpawner.spawnInterval;
        }

        if(trashSpawner != null)
        {
            originalTrashSpawnRate = trashSpawner.spawnInterval;
        }
    }

    public void TriggerEvent(int eventID)
    {
        if(!isEventActive && eventDictionary.ContainsKey(eventID))
        {
            eventDictionary[eventID]();
        }
    }

    #region Événements
    IEnumerator TriggerJellyfishInvasion()
    {
        isEventActive = true;
        yield return StartCoroutine(StartEventTransition("Invasion de Méduses !", Color.red));
        
        fishSpawner.fishPrefabs = settings.jellyfishPrefabs;
        yield return new WaitForSeconds(eventDuration);
        ResetToDefault();
    }

    IEnumerator TriggerLionfishInvasion()
    {
        isEventActive = true;
        yield return StartCoroutine(StartEventTransition("Invasion de Poisson-Lions !", new Color(1, 0.5f, 0)));
        
        fishSpawner.fishPrefabs = settings.lionfishPrefabs;
        yield return new WaitForSeconds(eventDuration);
        ResetToDefault();
    }

    IEnumerator TriggerBarracudaInvasion()
    {
        isEventActive = true;
        yield return StartCoroutine(StartEventTransition("Invasion de Barracudas !", Color.blue));
        
        fishSpawner.fishPrefabs = settings.barracudaPrefabs;
        yield return new WaitForSeconds(eventDuration);
        ResetToDefault();
    }

    IEnumerator TriggerCoralFestival()
    {
        isEventActive = true;
        yield return StartCoroutine(StartEventTransition("Fête du Corail !", settings.festivalTextColor));
        
        fishSpawner.spawnInterval = originalFishSpawnRate / settings.festivalSpawnMultiplier;
        yield return new WaitForSeconds(eventDuration);
        ResetToDefault();
    }

    IEnumerator TriggerTrashWave()
    {
        isEventActive = true;
        yield return StartCoroutine(StartEventTransition("Vague de Déchets !", settings.trashTextColor));
        
        // Modifie les deux spawners
        fishSpawner.spawnInterval = originalFishSpawnRate * settings.trashWaveFishReduction;
        trashSpawner.spawnInterval = originalTrashSpawnRate / settings.trashWaveSpawnIncrease;
        yield return new WaitForSeconds(eventDuration);
        ResetToDefault();
    }
    #endregion

    #region Transitions
    IEnumerator StartEventTransition(string message, Color color)
    {
        eventText.color = color;
        StartCoroutine(FadeInText(message));
        yield return new WaitForSeconds(textFadeInDuration + textVisibleDuration + textFadeOutDuration);
    }

    IEnumerator FadeInText(string message)
    {
        eventText.text = message;
        eventText.gameObject.SetActive(true);
        
        // Fade in
        float timer = 0f;
        while(timer < textFadeInDuration)
        {
            eventText.color = new Color(eventText.color.r, eventText.color.g, eventText.color.b, 
                                      Mathf.Lerp(0f, 1f, timer/textFadeInDuration));
            timer += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(textVisibleDuration);
        
        // Fade out
        timer = 0f;
        while(timer < textFadeOutDuration)
        {
            eventText.color = new Color(eventText.color.r, eventText.color.g, eventText.color.b, 
                                      Mathf.Lerp(1f, 0f, timer/textFadeOutDuration));
            timer += Time.deltaTime;
            yield return null;
        }
        
        eventText.gameObject.SetActive(false);
    }

    IEnumerator FadeInOverlay()
    {
        blackOverlayRenderer.gameObject.SetActive(true);
        Color c = blackOverlayRenderer.color;
        
        float timer = 0f;
        while(timer < overlayFadeInDuration)
        {
            c.a = Mathf.Lerp(0f, settings.overlayMaxOpacity, timer/overlayFadeInDuration);
            blackOverlayRenderer.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
    }
    #endregion

    #region Reset System
    void ResetToDefault()
    {
        // Réinitialisation des spawners
        if(fishSpawner != null)
        {
            fishSpawner.fishPrefabs = originalFishPrefabs;
            fishSpawner.spawnInterval = originalFishSpawnRate;
        }
        
        if(trashSpawner != null)
        {
            trashSpawner.spawnInterval = originalTrashSpawnRate;
        }
        
        StartCoroutine(FadeOutOverlay());
        isEventActive = false;
    }

    IEnumerator FadeOutOverlay()
    {
        Color c = blackOverlayRenderer.color;
        float startAlpha = c.a;
        
        float timer = 0f;
        while(timer < settings.overlayFadeOutDuration)
        {
            c.a = Mathf.Lerp(startAlpha, 0f, timer/settings.overlayFadeOutDuration);
            blackOverlayRenderer.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
        
        blackOverlayRenderer.gameObject.SetActive(false);
    }
    #endregion

    public void ForceStopEvent()
    {
        StopAllCoroutines();
        ResetToDefault();
    }
}
