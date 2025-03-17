using UnityEngine;

public class E_WallSpawner : MonoBehaviour
{
    public GameObject wallPrefab; // Prefab du mur à instancier
    public float minSpawnInterval = 3f; // Intervalle minimal de spawn
    public float maxSpawnInterval = 7f; // Intervalle maximal de spawn
    public float spawnRangeY = 3f; // Plage verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;
    private float currentSpawnInterval;
    private Camera mainCamera;
    private float screenRightEdge;

    void Start()
    {
        // Obtenir la caméra principale pour déterminer les limites de l'écran
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculer la position du bord droit de l'écran
            screenRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        }

        // Définir un premier intervalle de spawn aléatoire
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= currentSpawnInterval)
        {
            SpawnWall();
            timer = 0f;
            // Choisir un nouvel intervalle de spawn aléatoire pour la prochaine instance
            currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnWall()
    {
        // Calculer une position aléatoire sur l'axe Y dans la plage spécifiée
        float randomY = Random.Range(-spawnRangeY, spawnRangeY);

        // Position de spawn à droite de l'écran
        Vector3 spawnPosition = new Vector3(screenRightEdge + spawnXOffset, randomY - 3.2f, 0f);

        // Instancier le mur
        Instantiate(wallPrefab, spawnPosition, Quaternion.identity);
    }
}
