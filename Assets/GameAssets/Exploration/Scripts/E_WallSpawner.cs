using UnityEngine;

public class E_WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab; // Prefab du mur à instancier
    public float minSpawnInterval = 3f; // Intervalle minimal de spawn
    public float maxSpawnInterval = 7f; // Intervalle maximal de spawn
    public float spawnRangeY = 3f; // Plage verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;
    private float gameTimer = 0f; // Nouveau timer pour compter le temps de jeu
    private float currentSpawnInterval;
    private Camera mainCamera;
    private float screenRightEdge;
    private bool canSpawn = false; // Variable pour indiquer quand commencer le spawn

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            screenRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        }

        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        // Mettre à jour le temps de jeu
        gameTimer += Time.deltaTime;

        // Vérifier si 10 secondes se sont écoulées avant d'activer le spawn
        if (!canSpawn && gameTimer >= 10f)
        {
            canSpawn = true; // Autoriser le spawn après 10 secondes
            timer = 0f; // Réinitialiser le timer de spawn
        }

        if (canSpawn)
        {
            timer += Time.deltaTime;
            if (timer >= currentSpawnInterval)
            {
                SpawnWall();
                timer = 0f;
                currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    void SpawnWall()
    {
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(screenRightEdge + spawnXOffset, randomY - 3.2f, 0f);
        Instantiate(wallPrefab, spawnPosition, Quaternion.identity);
    }
}
