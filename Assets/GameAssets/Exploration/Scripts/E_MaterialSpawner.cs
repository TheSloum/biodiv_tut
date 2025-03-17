using UnityEngine;

public class E_MaterialSpawner : MonoBehaviour
{
    public GameObject woodPrefab;
    public GameObject stonePrefab;
    public GameObject ironPrefab;

    public float spawnInterval = 3f; // Intervalle de spawn en secondes
    public float spawnYMin = -4f;
    public float spawnYMax = 4f;

    private float timer = 0f;
    private Camera mainCamera;
    private float screenRightEdge;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            // Calculer la position de l'arrière-plan droit de l'écran
            screenRightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, mainCamera.nearClipPlane)).x;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnMaterial();
            timer = 0f;
        }
    }

    void SpawnMaterial()
    {
        // Choisir aléatoirement le type de matériau à spawn
        int randomType = Random.Range(0, 3); // 0: Bois, 1: Pierre, 2: Fer
        GameObject prefabToSpawn = null;
        int type = 0;

        switch (randomType)
        {
            case 0:
                prefabToSpawn = woodPrefab;
                type = 0; // Bois
                break;
            case 1:
                prefabToSpawn = stonePrefab;
                type = 1; // Pierre
                break;
            case 2:
                prefabToSpawn = ironPrefab;
                type = 2; // Fer
                break;
        }

        if (prefabToSpawn != null)
        {
            // Définir une position aléatoire sur l'axe Y
            float spawnY = Random.Range(spawnYMin, spawnYMax);
            Vector3 spawnPosition = new Vector3(screenRightEdge + 1f, spawnY, 0f);

            // Instancier le matériau
            GameObject material = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

            // Assigner le type de matériau
            E_Material materialScript = material.GetComponent<E_Material>();
            if (materialScript != null)
            {
                materialScript.materialType = type;
            }
        }
        else
        {
            Debug.LogWarning("Prefab de matériau non assigné dans le E_MaterialSpawner.");
        }
    }
}
