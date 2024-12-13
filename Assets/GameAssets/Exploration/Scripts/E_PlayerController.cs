using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    [Header("Configuration du Joueur")]
    public float moveForce = 200f; // Force appliquée pour le mouvement
    public float maxSpeed = 5f;    // Vitesse maximale du joueur
    public Rigidbody2D rb;

    private Vector2 movement;

    void Update()
    {
        // Gérer les entrées du joueur
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
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

            Debug.Log("Joueur poussé par le mur !");
        }
    }
}
