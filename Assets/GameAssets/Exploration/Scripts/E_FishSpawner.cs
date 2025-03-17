using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_FishSpawner : MonoBehaviour
{
    #region Singleton Instance
    public static E_FishSpawner Instance;
    #endregion

    [Header("Préfabriqués de Poissons")]
    [Tooltip("Liste des préfabriqués de différentes races de poissons.")]
    public GameObject[] fishPrefabs;

    [Header("Paramètres de Spawn")]
    [Tooltip("Intervalle de spawn en secondes.")]
    public float spawnInterval = 3f;
    [Tooltip("Variation verticale pour le spawn.")]
    public float spawnRangeY = 3f;
    [Tooltip("Distance à droite de l'écran pour le spawn.")]
    public float spawnXOffset = 10f;

    [Header("Paramètres Festival")]
    [Tooltip("Multiplicateur de réduction de l'intervalle pendant le festival.")]
    public float festivalSpawnMultiplier = 1.5f;

    private float timer = 0f;
    private float defaultSpawnInterval;

    private float sum = 0f;
    private List<float> weights = new List<float>();

    // Mode invasion : si activé, tous les poissons proviennent d'un prefab spécifique.
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
        defaultSpawnInterval = spawnInterval;
        
        // Calcul des poids pour la sélection pondérée.
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
        if (timer >= spawnInterval)
        {
            SpawnFish();
            timer = 0f;
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
            // Sélection pondérée d'un poisson à spawn.
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

    // Méthode pour activer le mode invasion.
    public void EnableInvasionMode(GameObject invasionFish)
    {
        invasionModeActive = true;
        invasionPrefab = invasionFish;
        Debug.Log("Mode invasion activé : tous les poissons seront remplacés par le prefab d'invasion.");
    }

    // Méthode pour désactiver le mode invasion.
    public void DisableInvasionMode()
    {
        invasionModeActive = false;
        invasionPrefab = null;
        Debug.Log("Mode invasion désactivé : retour au spawn de poissons normal.");
    }

    // Réinitialise le spawner selon les paramètres par défaut des événements.
    public void ResetToDefault(E_EventSettings settings)
    {
        fishPrefabs = settings.defaultFishPrefabs;
        spawnInterval = settings.defaultFishSpawnRate - Random.Range(0f, 10f);
        defaultSpawnInterval = spawnInterval;
    }

    // Méthode appelée pour augmenter le taux de spawn (exécutée au début de la Fête du Corail)
    public void IncreaseFishSpawnRate()
    {
        spawnInterval = defaultSpawnInterval / festivalSpawnMultiplier;
        Debug.Log("Fête du Corail : Taux de spawn augmenté.");
    }

    public void ReduceFishSpawnRate()
    {
        spawnInterval = defaultSpawnInterval * festivalSpawnMultiplier;
        Debug.Log("Vague de déchets : Taux de spawn réduit.");
    }

    public void ActivateTrashWaveEffect()
    {
        spawnInterval = defaultSpawnInterval * 2f;
        Debug.Log("Intervalle de spawn des poissons réduit (spawn moins rapide).");
    }


    // Méthode appelée pour restaurer le taux de spawn par défaut (exécutée à la fin de la Fête du Corail)
    public void RestoreDefaultSpawnRate()
    {
        spawnInterval = defaultSpawnInterval;
        Debug.Log("Fête du Corail : Taux de spawn restauré.");
    }


}
