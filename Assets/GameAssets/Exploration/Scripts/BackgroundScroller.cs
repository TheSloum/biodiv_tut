using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f; // Vitesse de défilement
    public float backgroundWidth = 10f; // Largeur du sprite de fond

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Déplacer le fond vers la gauche
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, backgroundWidth);
        transform.position = startPosition + Vector3.left * newPosition;
    }
}
