using UnityEngine;

public class E_OceanCurrent : MonoBehaviour
{
    [Header("Configuration du Courant")]
    [Tooltip("Direction du courant en degrés (0-360). 0° pointe vers la droite.")]
    [Range(0f, 360f)]
    public float directionDegrees = 0f;

    [Tooltip("Force de poussée appliquée au joueur.")]
    public float pushForce = 1000f;

    [Tooltip("Vitesse de défilement du courant.")]
    public float scrollSpeed = 5f;

    private Vector2 pushDirection;

    void Start()
    {
        InitializeCurrent();
    }

    public void InitializeCurrent()
    {
        // Convertir la direction en radians pour le calcul
        float directionRadians = directionDegrees * Mathf.Deg2Rad;

        // Calculer la direction du push en vecteur
        pushDirection = new Vector2(Mathf.Cos(directionRadians), Mathf.Sin(directionRadians)).normalized;

        // Appliquer la rotation au sprite pour qu'il pointe dans la direction du courant
        transform.rotation = Quaternion.Euler(0f, 0f, directionDegrees);
    }

    void Update()
    {
        // Déplacer le courant vers la gauche dans l'espace mondial
        transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime, Space.World);
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        // Appliquer la force continuellement tant que le joueur est dans le courant
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.AddForce(pushDirection * pushForce * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }


    void OnBecameInvisible()
    {
        // Détruire le courant lorsqu'il sort de l'écran pour optimiser les performances
        Destroy(gameObject);
    }
}
