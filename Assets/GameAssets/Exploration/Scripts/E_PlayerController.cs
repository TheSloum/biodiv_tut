using UnityEngine;

public class E_PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 movement;

    void Update()
    {
        // Entrées du joueur
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Déplacement du joueur
        transform.Translate(movement * moveSpeed * Time.fixedDeltaTime);
    }
}
