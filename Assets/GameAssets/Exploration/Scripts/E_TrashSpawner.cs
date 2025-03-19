using UnityEngine;

public class E_TrashSpawner : MonoBehaviour
{
    public static E_TrashSpawner Instance;

    [Header("Préfabriqué et paramètres de Trash")]
    public GameObject[] trashPrefabs;
    public float spawnInterval = 3f;
    public float spawnRangeY = 3f;
    public float spawnXOffset = 10f;
    public float minSpawnInterval = 4f;
    public float maxSpawnInterval = 8f;
    public float maxIntervalLimit = 20f;

    private float timer = 0f;
    private float defaultSpawnInterval;
    private Camera mainCamera;
    private float screenRightEdge;

    private float targetSpawnInterval;
    private float intervalIncreaseSpeed = 0.1f;

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
            screenRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        }

        // Initialiser le targetSpawnInterval
        targetSpawnInterval = maxSpawnInterval;
    }

    void Update()
    {
        AdjustSpawnRateBasedOnMaterials();

        // Mettre à jour l'intervalle cible de spawn (incrémentation progressive)
        if (spawnInterval < targetSpawnInterval)
        {
            spawnInterval += intervalIncreaseSpeed * Time.deltaTime;
        }
        else if (spawnInterval > targetSpawnInterval)
        {
            spawnInterval = targetSpawnInterval;
        }

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnTrash();
            timer = 0f;
        }
    }

    void SpawnTrash()
    {
        if (trashPrefabs.Length == 0) return;

        // Sélectionner un prefab aléatoire
        GameObject selectedTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        // Calculer une position aléatoire sur l'axe Y dans la plage spécifiée
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(screenRightEdge + spawnXOffset, randomY, 0f);

        // Instancier le Trash sélectionné
        Instantiate(selectedTrash, spawnPosition, Quaternion.identity);
    }

    private void AdjustSpawnRateBasedOnMaterials()
    {
        // Si bar_2 est proche de 1, on commence avec un intervalle de spawn de base
        if (Materials.instance.bar_2 >= 0 && Materials.instance.bar_2 <= 1)
        {
            spawnInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, Materials.instance.bar_2);
            targetSpawnInterval = Mathf.Lerp(16f, maxIntervalLimit, Materials.instance.bar_2); // Le délai cible se règle en fonction de bar_2
        }
    }

    public void IncreaseTrashSpawnRate()
    {
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval / 2f);
        Debug.Log("Vague de déchets activée : spawn des Trash augmenté.");
    }

    public void ActivateTrashWaveEffect()
    {
        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval * 0.5f);
        Debug.Log("Vague de déchets activée : intervalle de spawn des trash diminué (spawn plus fréquent).");
    }

    public void RestoreDefaultTrashSpawnRate()
    {
        spawnInterval = defaultSpawnInterval;
        Debug.Log("Fin de la vague de déchets : spawn des Trash restauré.");
    }
}
