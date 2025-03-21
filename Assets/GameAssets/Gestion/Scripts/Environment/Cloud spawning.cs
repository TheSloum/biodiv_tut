using UnityEngine;

public class Cloudspawning : MonoBehaviour
{
     public GameObject prefab; // Reference to the prefab to spawn
    public float startX = -2730f;
    public float startY = 3600f;
    public float endX = 2600f;
    public float startYMin = -1400f;
    public float startYMax = 3600f;
    public float minSpeed = 0.5f;
    public float maxSpeed = 2f;

    private float spawnInterval;
    private float nextSpawnTime;

    void Awake(){

    }

    void Update()
    {
        // Get the current value of bar_2 from Materials.instance
        float barValue = Materials.instance.bar_2;

        // Determine the spawn interval based on barValue
        if (barValue >= 0.25f)
        {
            // Interpolate between 1.2 seconds and 12 seconds based on the bar value
            spawnInterval = Mathf.Lerp(30.5f, 60f, (1 - barValue) / (1 - 0.25f));
            
            // Randomize the spawn interval within a range (±0.6 seconds)
            spawnInterval += Random.Range(-1.6f, 1.6f);

            // Spawn a prefab if enough time has passed
            if (Time.time >= nextSpawnTime)
            {
                SpawnAndMovePrefab();
                nextSpawnTime = Time.time + spawnInterval;
            }
        }
    }

    void SpawnAndMovePrefab()
    {
        // Create a random y position for each prefab (if needed)
        float randomY = Random.Range(startYMin, startYMax);
        
        // Spawn the prefab at the starting position
        GameObject spawnedPrefab = Instantiate(prefab, new Vector3(startX, randomY, 0), Quaternion.identity);
        
        // Attach a mover script to the spawned prefab
        spawnedPrefab.AddComponent<PrefabMover>().SetMoveParameters(endX, Random.Range(minSpeed, maxSpeed));
    }
}

public class PrefabMover : MonoBehaviour
{
    private float targetX;
    private float speed;

    public void SetMoveParameters(float targetX, float speed)
    {
        this.targetX = targetX;
        this.speed = speed;
    }

    void Update()
    {
        // Move the prefab to the right
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Destroy prefab once it has reached the target position
        if (transform.position.x >= targetX)
        {
            Destroy(gameObject);
        }
    }
}
