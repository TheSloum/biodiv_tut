using UnityEngine;

public class E_Material : MonoBehaviour
{
    public float speed = 2f; 
    public float oxygenPenalty = 10f; 
    public GameObject collectParticlesPrefab;
    public int materialType; // 0 = Bois, 1 = Pierre, 2 = Fer

    private E_OxygenManager oxygenManager;
    private Camera mainCamera;
    private float screenLeftEdge;

    void Start()
    {
        if (E_GameManager.instance != null)
        {
            oxygenManager = E_GameManager.instance.GetOxygenManager();
        }

        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            screenLeftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x;
        }
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < screenLeftEdge - 1f)
        {
            Destroy(gameObject);
            if (oxygenManager != null)
            {
                oxygenManager.DecreaseOxygen(oxygenPenalty);
                Debug.Log("Oxygène perdu ! Matériau manqué.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Matériau collecté : Type {materialType}");

            if (E_AudioManager.instance != null)
            {
                E_AudioManager.instance.PlayPopSound();
            }

            if (collectParticlesPrefab != null)
            {
                Instantiate(collectParticlesPrefab, transform.position, Quaternion.identity);
                Debug.Log("Particules de collecte instanciées.");
            }

            if (Materials.instance != null)
            {
                Debug.Log($"Avant collecte: mat_0 = {Materials.instance.mat_0}, mat_1 = {Materials.instance.mat_1}, mat_2 = {Materials.instance.mat_2}");

                switch (materialType)
                {
                    case 0: // Bois
                        Materials.instance.AddWood(20);
                        break;
                    case 1: // Pierre
                        Materials.instance.AddStone(20);
                        break;
                    case 2: // Fer
                        Materials.instance.AddIron(20);
                        break;
                    default:
                        Debug.LogWarning("Type de matériau invalide !");
                        break;
                }

                Debug.Log($"Après collecte: mat_0 = {Materials.instance.mat_0}, mat_1 = {Materials.instance.mat_1}, mat_2 = {Materials.instance.mat_2}");
            }
            else
            {
                Debug.LogWarning("Materials.instance est null dans E_Material.OnTriggerEnter2D");
            }

            Destroy(gameObject);
            Debug.Log("GameObject de matériau détruit après collecte.");
        }
    }
}
