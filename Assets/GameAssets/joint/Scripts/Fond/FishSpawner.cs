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
        // Détruire l'ancien poisson s'il existe
        if (currentFish != null)
        {
            Destroy(currentFish);
        }

        // Si aucun préfabriqué n'est assigné, on arrête ici
        if (fishPrefabs.Length == 0)
        {
            return;
        }

        // Choisir un préfabriqué aléatoire de poisson
        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFish = fishPrefabs[randomIndex];

        // Calculer la position du spawn avec la hauteur définie et une position X aléatoire
        float randomX = Random.Range(spawnRangeX.x, spawnRangeX.y);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, -1f); // Z défini explicitement à -1

        Debug.Log("Fish Spawn Position: " + spawnPosition); // Log pour vérifier la position

        // Créer un nouveau poisson à la position donnée
        currentFish = Instantiate(selectedFish, spawnPosition, Quaternion.identity);

        // Ajouter le poisson au conteneur pour mieux organiser la hiérarchie
        currentFish.transform.SetParent(fishContainer, false);

        // Activer le poisson
        currentFish.SetActive(true);
    }

    bool IsFishOffScreen()
    {
        if (currentFish == null)
            return true;

        // Vérifier si le poisson est hors de l'écran
        Vector3 fishPos = Camera.main.WorldToViewportPoint(currentFish.transform.position);
        return fishPos.x < 0 || fishPos.x > 1 || fishPos.y < 0 || fishPos.y > 1;
    }
}
