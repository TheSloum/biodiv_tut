using UnityEngine;

public class E_Wall : MonoBehaviour
{
    public float speed = 2f; // Vitesse de déplacement vers la gauche
    private Camera mainCamera;
    private float screenLeftEdge;

    void Start()
    {
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
        // Déplacer le mur vers la gauche
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Vérifier si le mur est sorti de l'écran
        if (transform.position.x < screenLeftEdge - 10f) // Ajustez la valeur selon votre scène
        {
            Destroy(gameObject);
        }
    }
}
