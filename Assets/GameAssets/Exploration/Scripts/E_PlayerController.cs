using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    [Header("Configuration du Joueur")]
    public float moveForce = 200f;
    public float maxSpeed = 5f;
    public float rotationSmoothness = 360f; // Degrés par seconde pour la rotation

    [Header("Gestion du Temps")]
    public float gameSpeed = 1f; // Modifier dans l'Inspector pour ajuster le TimeScale

    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isDirectionBack = false;
    private Vector2 movement;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (spriteRenderer == null)
            Debug.LogError("SpriteRenderer non trouvé !");
        if (animator == null)
            Debug.LogError("Animator non trouvé !");
        else
            animator.updateMode = AnimatorUpdateMode.UnscaledTime; // Empêche l'animation d'être ralentie
    }

    void Update()
    {
        // Modifier le TimeScale sans affecter les cinématiques
        Time.timeScale = gameSpeed;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        AdjustSpriteOrientation();
    }

    void FixedUpdate()
    {
        // Appliquer une force indépendamment du Time.timeScale
        Vector2 force = movement.normalized * moveForce * Time.unscaledDeltaTime;
        rb.AddForce(force, ForceMode2D.Force);

        // Vérifier si la vitesse dépasse la limite max
        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;
            rb.AddForce(pushDirection * 5f, ForceMode2D.Impulse);
        }
    }

    void AdjustSpriteOrientation()
    {
        if (Time.timeScale == 0) return;

        // Gestion du flip horizontal
        if (movement.x < 0) spriteRenderer.flipX = true;
        else if (movement.x > 0) spriteRenderer.flipX = false;
        isDirectionBack = spriteRenderer.flipX;

        // Calcul de la rotation cible
        float targetZ = 0f;
        if (movement.y > 0)
            targetZ = isDirectionBack ? -10f : 10f;
        else if (movement.y < 0)
            targetZ = isDirectionBack ? 10f : -10f;

        // Interpolation fluide
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        float rotationStep = rotationSmoothness * Time.unscaledDeltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
    }
}
