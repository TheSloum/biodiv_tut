using UnityEngine;

public class E_TrashSpawner : MonoBehaviour
{
    public static E_TrashSpawner Instance;

    [Header("Préfabriqué et paramètres de Trash")]
    public GameObject trashPrefab; // Prefab du Trash à instancier
    public float spawnInterval = 3f; // Intervalle de spawn en secondes
    public float spawnRangeY = 3f;   // Plage verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;
    private float defaultSpawnInterval;
    private Camera mainCamera;
    private float screenRightEdge;

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
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculer la position de l'arrière-plan droite de l'écran
            screenRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTrash();
            timer = 0f;
        }
    }

    void SpawnTrash()
    {
        // Calculer une position aléatoire sur l'axe Y dans la plage spécifiée
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        // Position de spawn à droite de l'écran
        Vector3 spawnPosition = new Vector3(screenRightEdge + spawnXOffset, randomY, 0f);
        // Instancier le Trash
        Instantiate(trashPrefab, spawnPosition, Quaternion.identity);
    }

    // Augmente le taux de spawn des Trash en diminuant l'intervalle
    public void IncreaseTrashSpawnRate()
    {
        spawnInterval = defaultSpawnInterval / 2f; // Par exemple, la moitié de l'intervalle par défaut
        Debug.Log("Vague de déchets activée : spawn des Trash augmenté.");
    }

    public void ActivateTrashWaveEffect()
    {
        spawnInterval = defaultSpawnInterval * 0.5f;
        Debug.Log("Vague de déchets activée : intervalle de spawn des trash diminué (spawn plus fréquent).");
    }


    // Restaure le taux de spawn des Trash à la valeur par défaut
    public void RestoreDefaultTrashSpawnRate()
    {
        spawnInterval = defaultSpawnInterval;
        Debug.Log("Fin de la vague de déchets : spawn des Trash restauré.");
    }
}
