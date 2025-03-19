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
    private float intervalIncreaseSpeed = 1f;

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


        targetSpawnInterval = maxSpawnInterval;
    }

    void Update()
    {
        AdjustSpawnRateBasedOnMaterials();

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

        GameObject selectedTrash = trashPrefabs[Random.Range(0, trashPrefabs.Length)];

        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(screenRightEdge + spawnXOffset, randomY, 0f);

        Instantiate(selectedTrash, spawnPosition, Quaternion.identity);
    }

    private void AdjustSpawnRateBasedOnMaterials()
    {
        if (Materials.instance.bar_2 >= 0 && Materials.instance.bar_2 <= 1)
        {
            spawnInterval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, Materials.instance.bar_2);
            targetSpawnInterval = Mathf.Lerp(16f, maxIntervalLimit, Materials.instance.bar_2);
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
