using UnityEngine;

public class E_FishController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseSpeed = 2f; // Vitesse de base du poisson
    public float speedVariance = 1f; // Variance aléatoire de la vitesse
    public float amplitude = 1.5f; // Amplitude du mouvement vertical (augmentée)
    public float frequency = 1f; // Fréquence du mouvement vertical
    public float changeDirectionChance = 0.1f; // 10% de chance de changer de direction

    [Header("Rotation Settings")]
    public float rotationUpAngle = -15f; // Angle de rotation vers le haut
    public float rotationDownAngle = 15f; // Angle de rotation vers le bas
    public float rotationSpeed = 20f; // Vitesse de rotation (augmentée)
    public float rotationThreshold = 0.005f; // Seuil pour le mouvement vertical (réduit)

    [Header("Fish Settings")]
    public Fishes fishData; // Référence au ScriptableObject Fishes

    private float speed;
    private Vector3 startPosition;
    private float movementTimer;
    private bool movingLeft = true;
    private float previousY;

    private bool hasInteracted = false; // Indicateur pour s'assurer que l'interaction se produit une seule fois
    void Start()
    {
        if (fishData.is_unlocked)
        {
    E_Outline outline = GetComponent<E_Outline>();
            Destroy(outline);
        }
        if (fishData == null)
        {
            Debug.LogError("FishData n'est pas assigné à E_FishController !");
            return;
        }

        startPosition = transform.position;
        speed = baseSpeed + Random.Range(-speedVariance, speedVariance);
        movementTimer = Random.Range(0f, 2f * Mathf.PI);
        previousY = transform.position.y;
    }
    

    void Update()
    {
        if (fishData == null) return;

        // Déplacer le poisson
        Vector2 direction = movingLeft ? Vector2.left : Vector2.right;
        transform.Translate(direction * speed * Time.deltaTime);

        // Mouvement ondulatoire
        movementTimer += frequency * Time.deltaTime;
        float newY = Mathf.Sin(movementTimer) * amplitude;
        transform.position = new Vector3(transform.position.x, startPosition.y + newY, transform.position.z);

        // Calculer le mouvement vertical actuel
        float verticalMovement = transform.position.y - previousY;
        previousY = transform.position.y;

        // Définir l'angle de rotation en fonction du mouvement vertical
        float targetRotation = 0f;

        if (verticalMovement > rotationThreshold)
        {
            targetRotation = rotationUpAngle;
        }
        else if (verticalMovement < -rotationThreshold)
        {
            targetRotation = rotationDownAngle;
        }
        else
        {
            targetRotation = 0f;
        }

        // Interpoler la rotation pour un effet plus fluide
        float currentRotation = Mathf.LerpAngle(transform.eulerAngles.z, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);

        // Changer de direction aléatoirement
        if (Random.value < changeDirectionChance * Time.deltaTime)
        {
            ChangeDirection();
        }

        // Détruire le poisson s'il sort de l'écran
        if ((movingLeft && transform.position.x < Camera.main.transform.position.x - 10f) ||
            (!movingLeft && transform.position.x > Camera.main.transform.position.x + 10f))
        {
            Destroy(gameObject);
        }

        // Debug pour vérifier verticalMovement
        // Debug.Log("Vertical Movement: " + verticalMovement + " | Target Rotation: " + targetRotation);
    }

    void ChangeDirection()
    {
        movingLeft = !movingLeft;
        // Inverser le sprite pour refléter la nouvelle direction
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasInteracted || fishData == null) return;

        if (collision.CompareTag("Player")  && !Materials.instance.researchCentr)
        {
            // Tenter de déverrouiller le poisson
            bool justUnlocked = false;
            if (FishUnlock.Instance != null)
            {
                justUnlocked = FishUnlock.Instance.UnlockFish(fishData.fishID);
            }
            else
            {
                Debug.LogError("FishUnlock Instance n'est pas initialisée. Veuillez vous assurer que FishUnlock est correctement configuré.");
            }

            if (justUnlocked && !Materials.instance.researchCentr)
            {
                // Déclencher le flash seulement si le poisson vient d'être déverrouillé OU si le centre de recherche est construit
                E_FlashEffect.Instance.TriggerFlash();

                // Marquer que cette interaction a eu lieu pour éviter des interactions futures
                hasInteracted = true;

                // Optionnel : Ajouter d'autres effets, par exemple, détruire le poisson ou appliquer des effets au joueur
                // Destroy(gameObject);
            }
            else
            {
                // Le poisson est déjà déverrouillé, ne rien faire
                Debug.Log($"Poisson avec ID {fishData.fishID} est déjà déverrouillé. Aucun flash déclenché.");
            }
        }
    }
}
