using UnityEngine;

public class E_BubbleSpawner : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnInterval = 2f;
    public float spawnRangeY = 3f;
    public float spawnXOffset = 10f; // Distance à droite de l'écran pour le spawn

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= spawnInterval)
        {
            SpawnBubble();
            timer = 0f;
        }
    }

    void SpawnBubble()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, Random.Range(-spawnRangeY, spawnRangeY), 0f);
        Instantiate(bubblePrefab, spawnPosition, Quaternion.identity);
    }
}
