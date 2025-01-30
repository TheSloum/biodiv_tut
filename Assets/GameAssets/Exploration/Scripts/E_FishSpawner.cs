using UnityEngine;

public class E_FishSpawner : MonoBehaviour
{
    [Header("Préfabriqués de Poissons")]
    [Tooltip("Liste des préfabriqués de différentes races de poissons.")]
    public GameObject[] fishPrefabs; // Liste de préfabriqués de poissons à instancier

    [Header("Paramètres de Spawn")]
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

    public void ResetToDefault(E_EventSettings settings)
    {
        fishPrefabs = settings.defaultFishPrefabs;
        spawnInterval = settings.defaultFishSpawnRate;
    }

    void SpawnFish()
    {
        if (fishPrefabs == null || fishPrefabs.Length == 0)
        {
            Debug.LogWarning("La liste des préfabriqués de poissons est vide. Assignez au moins un préfabriqué dans l'inspecteur.");
            return;
        }

        // Choisir aléatoirement un préfabriqué de poisson dans la liste
        int randomIndex = Random.Range(0, fishPrefabs.Length);
        GameObject selectedFishPrefab = fishPrefabs[randomIndex];

        // Calculer une position de spawn aléatoire sur l'axe Y
        float spawnY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(transform.position.x + spawnXOffset, spawnY, 0f);

        // Instancier le poisson
        GameObject fish = Instantiate(selectedFishPrefab, spawnPosition, Quaternion.identity);

        // Vérifier si le prefab possède un SpriteRenderer
        SpriteRenderer sr = fish.GetComponent<SpriteRenderer>();
        /*
        if(sr != null)
        {
            // Définir un Order in Layer aléatoire entre -6 et -2 (inclus)
            int sortingOrder = Random.Range(-1, -1); // -6, -5, -4, -3, -2
            sr.sortingOrder = sortingOrder;

            // Calculer l'échelle en fonction de l'Order in Layer
            // Plus l'ordre est bas, plus l'échelle est petite
            float scale = 3f + ((sortingOrder + 6) * 0.5f); // -6 => 3, -5 => 3.5, ..., -2 => 5
            fish.transform.localScale = Vector3.one * scale;
        }
        else
        {
            Debug.LogWarning("Le préfabriqué de poisson instancié n'a pas de SpriteRenderer attaché.");
        }
    */
    }
}
