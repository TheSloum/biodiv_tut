using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public Transform fishContainer; 
    public float spawnHeight = -1f;
    public Vector2 spawnRangeX = new Vector2(-50f, 50f);

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
            return;
        }
        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFish = fishPrefabs[randomIndex];
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, -1f);
        currentFish = Instantiate(selectedFish, spawnPosition, Quaternion.identity);
        currentFish.transform.SetParent(fishContainer, false);
        currentFish.SetActive(true);
    }

    bool IsFishOffScreen()
    {
        if (currentFish == null)
            return true;
        Vector3 fishPos = Camera.main.WorldToViewportPoint(currentFish.transform.position);
        return fishPos.x < 0 || fishPos.x > 1 || fishPos.y < 0 || fishPos.y > 1;
    }
}
