using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_FishSpawner : MonoBehaviour
{
    #region Singleton Instance
    public static E_FishSpawner Instance;
    #endregion

    [Header("Préfabriqués de Poissons")]
    public GameObject[] fishPrefabs;

    [Header("Paramètres de Spawn")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 5f;
    public float spawnRangeY = 3f;
    public float spawnXOffset = 10f;

    [Header("Paramètres Festival")]
    public float festivalSpawnMultiplier = 1.5f;

    private float timer = 0f;
    private float currentSpawnInterval;
    private float sum = 0f;
    private List<float> weights = new List<float>();

    public bool invasionModeActive = false;
    public GameObject invasionPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);

        foreach (GameObject prefab in fishPrefabs)
        {
            E_FishController fishScript = prefab.GetComponent<E_FishController>();
            if (fishScript != null)
            {
                weights.Add(fishScript.fishData.freqWeight);
            }
        }

        sum = 0f;
        foreach (float value in weights)
        {
            sum += value;
        }

        // Appliquer l'effet de l'événement actif (s'il y en a un) au démarrage de la scène Exploration.
        ApplyActiveEventEffect();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentSpawnInterval)
        {
            SpawnFish();
            timer = 0f;
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnFish()
    {
        GameObject prefabToSpawn = null;
        if (invasionModeActive && invasionPrefab != null)
        {
            prefabToSpawn = invasionPrefab;
        }
        else
        {
            float cumulativeWeight = 0f;
            float randomValue = Random.Range(0f, sum);
            int tospawn = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                cumulativeWeight += weights[i];
                if (randomValue < cumulativeWeight)
                {
                    tospawn = i;
                    break;
                }
            }
            prefabToSpawn = fishPrefabs[tospawn];
        }

        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, transform.position.y + spawnY, 0f);
        Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }

    public void EnableInvasionMode(GameObject invasionFish)
    {
        invasionModeActive = true;
        invasionPrefab = invasionFish;
        Debug.Log("Mode invasion activé : tous les poissons seront remplacés par le prefab d'invasion.");
    }

    public void DisableInvasionMode()
    {
        invasionModeActive = false;
        invasionPrefab = null;
        Debug.Log("Mode invasion désactivé : retour au spawn de poissons normal.");
    }

    public void ResetToDefault(E_EventSettings settings)
    {
        fishPrefabs = settings.defaultFishPrefabs;
        minSpawnInterval = settings.defaultFishSpawnRate - Random.Range(0f, 5f);
        maxSpawnInterval = settings.defaultFishSpawnRate + Random.Range(0f, 5f);
    }

    public void IncreaseFishSpawnRate()
    {
        minSpawnInterval /= festivalSpawnMultiplier;
        maxSpawnInterval /= festivalSpawnMultiplier;
        Debug.Log("Effet événement : Taux de spawn de poissons augmenté.");
    }

    public void ReduceFishSpawnRate()
    {
        minSpawnInterval *= festivalSpawnMultiplier;
        maxSpawnInterval *= festivalSpawnMultiplier;
        Debug.Log("Effet événement : Taux de spawn de poissons réduit.");
    }

    public void ActivateTrashWaveEffect()
    {
        minSpawnInterval *= 2f;
        maxSpawnInterval *= 2f;
        Debug.Log("Effet événement : Intervalle de spawn des poissons augmenté (spawn moins rapide).");
    }

    public void RestoreDefaultSpawnRate()
    {
        minSpawnInterval = 2f;
        maxSpawnInterval = 5f;
        Debug.Log("Effet événement terminé : Taux de spawn restauré.");
    }

    void ApplyActiveEventEffect()
    {
        if (E_Event.activeEventID == -1)
            return;
            
        // Vérifier si l'événement actif est une invasion (IDs 2, 4, 5, 12)
        if (E_Event.activeEventID == 2 || E_Event.activeEventID == 4 || E_Event.activeEventID == 5 || E_Event.activeEventID == 12)
        {
            E_Event eventScript = FindObjectOfType<E_Event>();
            if (eventScript != null && eventScript.eventSettings != null)
            {
                InvasionType invasionEvent = eventScript.eventSettings.invasionTypes.Find(e => e.eventID == E_Event.activeEventID);
                if (invasionEvent != null && invasionEvent.prefabs.Length > 0)
                {
                    EnableInvasionMode(invasionEvent.prefabs[0]);
                    Debug.Log("Mode invasion rétabli sur E_FishSpawner pour l'événement : " + invasionEvent.name);
                }
                else
                {
                    Debug.LogWarning("Aucun prefab d'invasion trouvé pour l'événement actif (ID: " + E_Event.activeEventID + ").");
                }
            }
        }
        else
        {
            switch (E_Event.activeEventID)
            {
                case 0: // Vague de déchets
                    ActivateTrashWaveEffect();
                    break;
                case 1: // Marée noire
                    IncreaseFishSpawnRate();
                    break;
                case 3: // Fête du corail
                    IncreaseFishSpawnRate();
                    break;
                case 9: // Pêche illégale
                    minSpawnInterval *= 1.5f;
                    maxSpawnInterval *= 1.5f;
                    Debug.Log("Effet événement : Taux de spawn de poissons réduit (Pêche illégale).");
                    break;
                case 10: // Canicule marine
                    minSpawnInterval *= 1.2f;
                    maxSpawnInterval *= 1.2f;
                    Debug.Log("Effet événement : Taux de spawn de poissons réduit (Canicule marine).");
                    break;
                default:
                    break;
            }
        }
    }
}
