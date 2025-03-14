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
    public float spawnInterval = 3f; // Intervalle de spawn en secondes
    public float spawnRangeY = 3f;   // Variation verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;
    private float defaultSpawnInterval;

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

    void Start()
    {
        defaultSpawnInterval = spawnInterval;
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

    void SpawnFish()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogWarning("La liste des préfabriqués de poissons est vide. Assignez au moins un préfabriqué dans l'inspecteur.");
            return;
        }

        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFishPrefab = fishPrefabs[randomIndex];

        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, spawnY, 0f);

        Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);
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
