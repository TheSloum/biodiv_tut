using UnityEngine;
using System.Collections.Generic;
public class E_FishSpawner : MonoBehaviour
{
    #region Singleton Instance
    public static E_FishSpawner Instance;
    #endregion

    [Header("Préfabriqués de Poissons")]
    [Tooltip("Liste des préfabriqués de différentes races de poissons.")]
    public GameObject[] fishPrefabs;

    [Header("Paramètres de Spawn")]
    public float spawnInterval = 3f; // Intervalle de spawn en secondes
    public float spawnRangeY = 3f;   // Variation verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;
    private float defaultSpawnInterval;


    private float totalWeight;
    private int tospawn;
        private float sum = 0f;
    void Awake()
    {
        // Initialisation du singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    List<float> weights = new List<float>();
    private float cumulativeWeight;
    private float randomValue;
    void Start()
    {
        defaultSpawnInterval = spawnInterval;
        

        foreach (GameObject prefab in fishPrefabs)
        {
            E_FishController fishScript = prefab.GetComponent<E_FishController>();

            if (fishScript != null)
            {
                Fishes fish = fishScript.fishData;
                weights.Add(fish.freqWeight);
            }
        }
sum = 0f;
foreach (float value in weights)
{
    sum += value;
}
    }

    void SpawnFish()
    {
        cumulativeWeight = 0f;
float randomValue = Random.Range(0f, sum);
Debug.Log("Random de: " + randomValue + " - " + sum);
        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            Debug.Log(i + "AFJV");
            if (randomValue < cumulativeWeight)
            {
                tospawn = i;
                break;
            }
        }
        GameObject selectedFishPrefab = fishPrefabs[tospawn];
        Debug.Log("Spawn de " + fishPrefabs[tospawn] + " - " + tospawn);

        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, spawnY, 0f);

        Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnInterval)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    public void ResetToDefault(E_EventSettings settings)
    {
        fishPrefabs = settings.defaultFishPrefabs;
        spawnInterval = settings.defaultFishSpawnRate;
        defaultSpawnInterval = spawnInterval;
    }

    

    // Méthode appelée lors du début de l'événement "Pêche illégale" pour réduire le taux de spawn (augmentation de l'intervalle)
    public void ReduceFishSpawnRate()
    {
        // Par exemple, doubler l'intervalle de spawn pour réduire le nombre de poissons
        spawnInterval = defaultSpawnInterval * 2f;
        Debug.Log("Pêche illégale activée : taux de spawn de poissons réduit.");
    }

    // Méthode appelée à la fin de l'événement pour restaurer le taux de spawn par défaut
    public void RestoreDefaultSpawnRate()
    {
        spawnInterval = defaultSpawnInterval;
        Debug.Log("Fin de la pêche illégale : taux de spawn de poissons restauré.");
    }
}
