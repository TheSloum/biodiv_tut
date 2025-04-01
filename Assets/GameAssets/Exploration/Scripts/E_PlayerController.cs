using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    [Header("Configuration du Joueur")]
    public float moveForce = 200f;
    public float maxSpeed = 5f;
    public float rotationSmoothness = 360f; // Degrés par seconde pour la rotation

    [Header("Gestion du Temps")]
    private float gameTimer = 0f; // Temps écoulé
    public float maxTimeScale = 10f; // Vitesse max du jeu
    public float accelerationRate = 0.5f; // Facteur d’accélération

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
        // Incrémenter le temps de jeu
        gameTimer += Time.deltaTime;

        // Appliquer la vitesse du jeu en fonction du temps écoulé
        AdjustGameSpeed();

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        AdjustSpriteOrientation();
    }

    void FixedUpdate()
    {
        Vector2 force = movement.normalized * moveForce * Time.unscaledDeltaTime;
        rb.AddForce(force, ForceMode2D.Force);

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    void LateUpdate()
    {
        CheckOutOfBounds();
    }

    /// <summary>
    /// Augmente progressivement la vitesse du jeu à partir de 2 minutes.
    /// </summary>
    void AdjustGameSpeed()
    {
        if (gameTimer >= 90f) // Commence à accélérer après 2 minutes
        {
            float targetTimeScale = 1f + ((gameTimer - 90f) / 60f) * accelerationRate; // Augmente progressivement
            Debug.Log(targetTimeScale);
            // Limite la vitesse max
            Time.timeScale = Mathf.Min(targetTimeScale, maxTimeScale);
        }
    }

    void CheckOutOfBounds()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            Vector3 newViewportPos = new Vector3(0.5f, 0.5f, Mathf.Abs(Camera.main.transform.position.z - transform.position.z));
            Vector3 newWorldPos = Camera.main.ViewportToWorldPoint(newViewportPos);
            transform.position = new Vector3(newWorldPos.x, newWorldPos.y, transform.position.z);
        }
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

        if (movement.x < 0) spriteRenderer.flipX = true;
        else if (movement.x > 0) spriteRenderer.flipX = false;
        isDirectionBack = spriteRenderer.flipX;

        float targetZ = 0f;
        if (movement.y > 0)
            targetZ = isDirectionBack ? -10f : 10f;
        else if (movement.y < 0)
            targetZ = isDirectionBack ? 10f : -10f;

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        float rotationStep = rotationSmoothness * Time.unscaledDeltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
    }
}
