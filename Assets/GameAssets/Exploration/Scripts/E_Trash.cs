using UnityEngine;

public class E_Trash : MonoBehaviour
{
    public float speed = 2f; // Vitesse de déplacement vers la gauche
    public float oxygenPenalty = 10f; // Oxygène perdu si le Trash est manqué
    public GameObject collectParticlesPrefab; // Prefab des particules lors de la collecte

    private E_OxygenManager oxygenManager;
    private Camera mainCamera;
    private float screenLeftEdge;

    void Start()
    {
        // Obtenir la référence au gestionnaire d'oxygène via le GameManager
        oxygenManager = E_GameManager.instance.GetOxygenManager();

        // Obtenir la caméra principale pour déterminer les limites de l'écran
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculer la position de l'arrière-plan gauche de l'écran
            screenLeftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x;
        }
    }

    void Update()
    {
        // Déplacer le Trash vers la gauche
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Vérifier si le Trash est sorti de l'écran
        if (transform.position.x < screenLeftEdge - 1f) // Ajouter une marge pour s'assurer qu'il est bien sorti
        {
            // Détruire le Trash et pénaliser l'oxygène
            Destroy(gameObject);
            if (oxygenManager != null)
            {
                oxygenManager.DecreaseOxygen(oxygenPenalty);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (E_AudioManager.instance != null)
            {
                E_AudioManager.instance.PlayPopSound();
            }
            // Instancier les particules de collecte
            if (collectParticlesPrefab != null)
            {
                Instantiate(collectParticlesPrefab, transform.position, Quaternion.identity);
            }

            // Incrémenter le compteur de Trash collectés
            if (oxygenManager != null)
            {
                oxygenManager.IncrementTrashCollected();
            }

            // Détruire le Trash après collecte
            Destroy(gameObject);
        }
    }
}
