using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    private GameObject currentFish;
    private float timeToSpawn = 3f;
    private float spawnTimer = 0f;

    void Start()
    {
        SpawnNewFish();
    }

    void Update()
    {
        if (currentFish == null || IsFishOffScreen())
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= timeToSpawn)
            {
                SpawnNewFish();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnNewFish()
    {
        if (currentFish != null)
        {
            Destroy(currentFish);
        }

        if (fishPrefabs.Length == 0)
        {
            Debug.LogWarning("Aucun prefab de poisson assigné dans FishSpawner.");
            return;
        }

        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFish = fishPrefabs[randomIndex];

        currentFish = Instantiate(selectedFish, Vector3.zero, Quaternion.identity);

        currentFish.SetActive(true);

        Debug.Log($"Fish instantiated at position: {currentFish.transform.position}");
    }

    bool IsFishOffScreen()
    {
        Vector3 fishPos = Camera.main.WorldToViewportPoint(currentFish.transform.position);
        return fishPos.x < 0 || fishPos.x > 1 || fishPos.y < 0 || fishPos.y > 1;
    }
}
