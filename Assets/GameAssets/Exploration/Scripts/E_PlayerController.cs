using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
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
        // Appliquer le mouvement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
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
