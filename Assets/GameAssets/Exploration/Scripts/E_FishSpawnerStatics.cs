using UnityEngine;
using System.Collections.Generic;

public class E_FishSpawnerStatics : MonoBehaviour
{
    #region Singleton Instance
    public static E_FishSpawnerStatics Instance;
    #endregion

    [Header("Préfabriqués de Poissons")]
    [Tooltip("Liste des préfabriqués de différentes races de poissons.")]
    public GameObject[] fishPrefabs;

    [Header("Paramètres de Spawn")]
    public float minSpawnInterval = 2f; // Intervalle minimal
    public float maxSpawnInterval = 5f; // Intervalle maximal
    public float spawnRangeY = 3f;
    public float spawnXOffset = 10f;

    private float timer = 0f;
    private float currentSpawnInterval;
    private float sum = 0f;
    private int tospawn;

    List<float> weights = new List<float>();

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
        float cumulativeWeight = 0f;
        float randomValue = Random.Range(0f, sum);

        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                tospawn = i;
                break;
            }
        }

        GameObject selectedFishPrefab = fishPrefabs[tospawn];

        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, transform.position.y + spawnY, 0f);

        Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);
    }

    public void ResetToDefault(E_EventSettings settings)
    {
        fishPrefabs = settings.defaultFishPrefabs;
        minSpawnInterval = settings.defaultFishSpawnRate - Random.Range(0f, 3f);
        maxSpawnInterval = settings.defaultFishSpawnRate + Random.Range(0f, 3f);
    }

    public void ReduceFishSpawnRate()
    {
        minSpawnInterval *= 2f;
        maxSpawnInterval *= 2f;
        Debug.Log("Pêche illégale activée : taux de spawn réduit.");
    }

    public void RestoreDefaultSpawnRate()
    {
        minSpawnInterval = 2f;
        maxSpawnInterval = 5f;
        Debug.Log("Fin de la pêche illégale : taux de spawn restauré.");
    }
}
