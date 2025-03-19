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
        Debug.Log("Fête du Corail : Taux de spawn augmenté.");
    }

    public void ReduceFishSpawnRate()
    {
        minSpawnInterval *= festivalSpawnMultiplier;
        maxSpawnInterval *= festivalSpawnMultiplier;
        Debug.Log("Vague de déchets : Taux de spawn réduit.");
    }

    public void ActivateTrashWaveEffect()
    {
        minSpawnInterval *= 2f;
        maxSpawnInterval *= 2f;
        Debug.Log("Intervalle de spawn des poissons réduit (spawn moins rapide).");
    }

    public void RestoreDefaultSpawnRate()
    {
        minSpawnInterval = 2f;
        maxSpawnInterval = 5f;
        Debug.Log("Fête du Corail : Taux de spawn restauré.");
    }
}
