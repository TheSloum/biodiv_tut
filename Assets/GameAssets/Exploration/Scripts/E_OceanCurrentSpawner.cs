using UnityEngine;

public class E_OceanCurrentSpawner : MonoBehaviour
{
    [Header("Configuration du Spawner")]
    [Tooltip("Préfabriqué du courant marin.")]
    public GameObject oceanCurrentPrefab;

    [Tooltip("Intervalle de spawn en secondes.")]
    public float spawnInterval = 5f;

    [Tooltip("Définir la plage Y pour le spawn des courants.")]
    public float spawnRangeY = 3f;

    [Tooltip("Offset X pour le spawn (distance à droite de l'écran).")]
    public float spawnXOffset = 10f;

    [Tooltip("Vitesse de défilement des courants (doit correspondre à celle des courants).")]
    public float scrollSpeed = 5f;

    [Header("Configuration de la Push Force")]
    [Tooltip("Valeur minimale de la pushForce.")]
    public float minPushForce = 800f;

    [Tooltip("Valeur maximale de la pushForce.")]
    public float maxPushForce = 1200f;

    private float spawnTimer = 0f;
    private float gameTimer = 0f; // Nouveau timer pour suivre le temps écoulé
    private bool canSpawn = false; // Indicateur pour autoriser le spawn

    void Update()
    {
        // Mettre à jour le temps de jeu
        gameTimer += Time.deltaTime;

        // Vérifier si les 10 secondes sont passées avant de permettre le spawn
        if (!canSpawn && gameTimer >= 26f)
        {
            canSpawn = true; // Activer le spawn après 10 secondes
            spawnTimer = 0f; // Réinitialiser le timer de spawn
        }

        // Si le spawn est activé, on lance le système de spawn
        if (canSpawn)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnOceanCurrent();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnOceanCurrent()
    {
        if (oceanCurrentPrefab == null)
        {
            Debug.LogError("[E_OceanCurrentSpawner] Aucun prefab de courant marin assigné !");
            return;
        }

        // Calculer la position de spawn
        float spawnPosX = transform.position.x + spawnXOffset;
        float spawnPosY = Random.Range(-spawnRangeY, spawnRangeY);
        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, 0f);

        // Instancier le courant marin
        GameObject current = Instantiate(oceanCurrentPrefab, spawnPosition, Quaternion.identity);

        // Vérifier si le script du courant marin est présent
        E_OceanCurrent currentScript = current.GetComponent<E_OceanCurrent>();
        if (currentScript != null)
        {
            // Définir une direction aléatoire
            float randomDirection = Random.Range(0f, 360f);
            currentScript.directionDegrees = randomDirection;
            currentScript.InitializeCurrent();

            // Définir la vitesse de défilement
            currentScript.scrollSpeed = scrollSpeed;

            // Définir une pushForce aléatoire entre minPushForce et maxPushForce
            currentScript.pushForce = Random.Range(minPushForce, maxPushForce);
        }
        else
        {
            Debug.LogError("[E_OceanCurrentSpawner] Le prefab de courant marin n'a pas de script E_OceanCurrent attaché !");
        }
    }
}
