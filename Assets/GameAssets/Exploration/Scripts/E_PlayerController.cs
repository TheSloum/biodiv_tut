using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    [Header("Configuration du Joueur")]
    public float moveForce = 200f; // Force appliquée pour le mouvement
    public float maxSpeed = 5f;    // Vitesse maximale du joueur
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer; // Référence au SpriteRenderer du personnage
    private bool isDirectionBack = false;
    private Vector2 movement;

    void Start()
    {
        // Assigner le SpriteRenderer automatiquement
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer non trouvé ! Assurez-vous que le SpriteRenderer est attaché à l'objet.");
        }
    }

    void Update()
    {
        // Gérer les entrées du joueur
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Appeler la méthode pour ajuster la rotation du sprite selon les déplacements
        AdjustSpriteOrientation();
    }

    void FixedUpdate()
    {
        // Appliquer une force basée sur les entrées utilisateur
        Vector2 force = movement.normalized * moveForce;
        rb.AddForce(force);

        // Limiter la vitesse maximale
        Vector2 clampedVelocity = rb.velocity;
        if (clampedVelocity.magnitude > maxSpeed)
        {
            rb.velocity = clampedVelocity.normalized * maxSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // Calculer la direction de la poussée
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;

            // Définir la force de poussée
            float pushForce = 5f; // Ajustez selon vos besoins

            // Appliquer la force de poussée au joueur
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }

    // Fonction pour ajuster l'orientation du sprite
    void AdjustSpriteOrientation()
    {
        // Vérifier si le temps est en pause (Time.timeScale == 0)
        if (Time.timeScale == 0)
        {
            return; // Ne rien faire si le temps est en pause
        }

        // Inverser le sprite horizontalement quand on se déplace à gauche
        if (movement.x < 0)
        {
            spriteRenderer.flipX = true; // Retourner le sprite horizontalement
            isDirectionBack = true;
        }
        else if (movement.x > 0)
        {
            spriteRenderer.flipX = false; // Réinitialiser l'orientation du sprite
            isDirectionBack = false;
        }

        // Incliner légèrement le sprite pour les mouvements verticaux
        if (movement.y > 0) // Déplacement vers le haut
        {
            if (isDirectionBack == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, -10); // Rotation légère vers le haut
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 10);
            }
        }
        else if (movement.y < 0) // Déplacement vers le bas
        {
            if (isDirectionBack == true)
            {
                transform.rotation = Quaternion.Euler(0, 0, 10); // Rotation légère vers le bas
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, -10);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0); // Réinitialiser la rotation
        }
    }

}
