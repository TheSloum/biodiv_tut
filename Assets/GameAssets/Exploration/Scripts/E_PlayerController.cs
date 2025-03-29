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

    void LateUpdate()
    {
        CheckOutOfBounds();
    }

    /// <summary>
    /// Vérifie si le joueur est en dehors de la zone visible de la caméra.
    /// Si c'est le cas, il est repositionné au centre de la vue.
    /// </summary>
    void CheckOutOfBounds()
    {
        // Convertir la position du joueur en coordonnées de la caméra (viewport)
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        // La zone visible se trouve entre 0 et 1 sur les axes x et y
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
        {
            // Repositionner le joueur au centre de la caméra
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

    /// <summary>
    /// Ajuste l'orientation du sprite en fonction des mouvements du joueur.
    /// </summary>
    void AdjustSpriteOrientation()
    {
        if (Time.timeScale == 0) return;

        // Gestion du flip horizontal
        if (movement.x < 0) spriteRenderer.flipX = true;
        else if (movement.x > 0) spriteRenderer.flipX = false;
        isDirectionBack = spriteRenderer.flipX;

        // Calcul de la rotation cible en fonction de la direction verticale
        float targetZ = 0f;
        if (movement.y > 0)
            targetZ = isDirectionBack ? -10f : 10f;
        else if (movement.y < 0)
            targetZ = isDirectionBack ? 10f : -10f;

        // Interpolation fluide vers la rotation cible
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetZ);
        float rotationStep = rotationSmoothness * Time.unscaledDeltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationStep);
    }
}
