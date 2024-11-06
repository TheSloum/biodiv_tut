using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    public GameObject bg1;
    public GameObject bg2;
    public float scrollSpeed = 2f;
    public float backgroundWidth = 10f; // Largeur du sprite de fond

    void Update()
    {
        // Déplacer les fonds vers la gauche
        bg1.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;
        bg2.transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        // Vérifier si BG_1 est complètement à gauche
        if (bg1.transform.position.x + backgroundWidth < 0)
        {
            bg1.transform.position = new Vector3(bg2.transform.position.x + backgroundWidth, bg1.transform.position.y, bg1.transform.position.z);
        }

        // Vérifier si BG_2 est complètement à gauche
        if (bg2.transform.position.x + backgroundWidth < 0)
        {
            bg2.transform.position = new Vector3(bg1.transform.position.x + backgroundWidth, bg2.transform.position.y, bg2.transform.position.z);
        }
    }
}
