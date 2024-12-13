using UnityEngine;

public class E_Material : MonoBehaviour
{
    public float speed = 2f; // Vitesse de déplacement vers la gauche
    public float oxygenPenalty = 10f; // Oxygène perdu si le matériau est manqué
    public GameObject collectParticlesPrefab; // Prefab des particules lors de la collecte

    // Type de matériau (0: Bois, 1: Pierre, 2: Fer)
    public int materialType; // 0 = Bois, 1 = Pierre, 2 = Fer

    private E_OxygenManager oxygenManager;
    private Camera mainCamera;
    private float screenLeftEdge;

    void Start()
    {
        // Obtenir la référence au gestionnaire d'oxygène via le GameManager
        if (E_GameManager.instance != null)
        {
            oxygenManager = E_GameManager.instance.GetOxygenManager();
            if (oxygenManager == null)
            {
                Debug.LogWarning("OxygenManager est null dans E_Material.Start()");
            }
            else
            {
                Debug.Log("OxygenManager initialisé dans E_Material.Start()");
            }
        }
        else
        {
            Debug.LogWarning("E_GameManager.instance est null dans E_Material.Start()");
        }

        // Obtenir la caméra principale pour déterminer les limites de l'écran
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculer la position de l'arrière-plan gauche de l'écran
            screenLeftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane)).x;
            Debug.Log($"screenLeftEdge défini à {screenLeftEdge}");
        }
        else
        {
            Debug.LogWarning("Main Camera non trouvée dans E_Material.Start()");
        }
    }

    void Update()
    {
        // Déplacer le matériau vers la gauche
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Vérifier si le matériau est sorti de l'écran
        if (transform.position.x < screenLeftEdge - 1f) // Ajouter une marge pour s'assurer qu'il est bien sorti
        {
            // Détruire le matériau et pénaliser l'oxygène
            Destroy(gameObject);
            if (oxygenManager != null)
            {
                oxygenManager.DecreaseOxygen(oxygenPenalty);
                Debug.Log("Oxygène perdu ! Matériau manqué.");
            }
            else
            {
                Debug.LogWarning("oxygenManager est null lors de la pénalité pour matériau manqué.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"Matériau collecté : Type {materialType}");

            // Jouer l'effet sonore 'pop'
            if (E_AudioManager.instance != null)
            {
                E_AudioManager.instance.PlayPopSound();
            }
            else
            {
                Debug.LogWarning("E_AudioManager.instance est null dans E_Material.OnTriggerEnter2D");
            }

            // Instancier les particules de collecte
            if (collectParticlesPrefab != null)
            {
                Instantiate(collectParticlesPrefab, transform.position, Quaternion.identity);
                Debug.Log("Particules de collecte instanciées.");
            }
            else
            {
                Debug.LogWarning("collectParticlesPrefab est null dans E_Material.OnTriggerEnter2D");
            }

            // Ajouter le matériau correspondant via Materials.cs
            if (Materials.instance != null)
            {
                Debug.Log($"Avant collecte: mat_0 (Bois) = {Materials.instance.mat_0}, mat_1 (Pierre) = {Materials.instance.mat_1}, mat_2 (Fer) = {Materials.instance.mat_2}");

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

                Debug.Log($"Après collecte: mat_0 (Bois) = {Materials.instance.mat_0}, mat_1 (Pierre) = {Materials.instance.mat_1}, mat_2 (Fer) = {Materials.instance.mat_2}");
            }
            else
            {
                Debug.LogWarning("Materials.instance est null dans E_Material.OnTriggerEnter2D");
            }

            // Détruire le matériau après collecte
            Destroy(gameObject);
            Debug.Log("GameObject de matériau détruit après collecte.");
        }
    }
}
