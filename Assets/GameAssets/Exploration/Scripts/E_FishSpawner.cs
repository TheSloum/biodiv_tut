using UnityEngine;

public class E_FishSpawner : MonoBehaviour
{
    public GameObject fishPrefab; // Prefab de poisson à instancier
    public float spawnInterval = 3f; // Intervalle de spawn en secondes
    public float spawnRangeY = 3f; // Variation verticale pour le spawn
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnInterval)
        {
            SpawnFish();
            timer = 0f;
        }
    }

    void SpawnFish()
    {
        // Calculer une position de spawn aléatoire sur l'axe Y
        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, spawnY, 0f);
        Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
    }
}
