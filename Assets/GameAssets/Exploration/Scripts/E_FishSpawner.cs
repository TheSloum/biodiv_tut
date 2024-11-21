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

        // Instancier le poisson
        GameObject fish = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);

        // Vérifier si le prefab possède un SpriteRenderer
        SpriteRenderer sr = fish.GetComponent<SpriteRenderer>();
        if(sr != null)
        {
            // Définir un Order in Layer aléatoire entre -6 et -2 (inclus)
            int sortingOrder = Random.Range(-6, -1); // -6, -5, -4, -3, -2
            sr.sortingOrder = sortingOrder;

            // Calculer l'échelle en fonction de l'Order in Layer
            // Plus l'ordre est bas, plus l'échelle est petite
            float scale = 3f + ((sortingOrder + 6) * 0.5f); // -6 => 3, -5 => 3.5, ..., -2 => 5
            fish.transform.localScale = Vector3.one * scale;
        }
        else
        {
            Debug.LogWarning("Le prefab de poisson n'a pas de SpriteRenderer attaché.");
        }
    }
}
