using UnityEngine;

public class E_OxygenBubble : MonoBehaviour
{
    public AudioClip sfxClip;
    public float speed = 2f; // Vitesse horizontale
    public float verticalSpeed = 0.5f; // Vitesse verticale
    public float oxygenAmount = 20f;
    private E_OxygenManager oxygenManager;

    void Start()
    {
        oxygenManager = E_GameManager.instance.GetOxygenManager();
    }

    void Update()
    {
        // Créer un vecteur de mouvement combinant gauche et haut
        Vector2 movement = Vector2.left * speed + Vector2.up * verticalSpeed;
        transform.Translate(movement * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            // Ajouter de l'oxygène
            if (oxygenManager != null)
            {
                oxygenManager.AddOxygen(oxygenAmount);
            }
            SoundManager.instance.PlaySFX(sfxClip);
            Destroy(gameObject);
        }
    }
}
