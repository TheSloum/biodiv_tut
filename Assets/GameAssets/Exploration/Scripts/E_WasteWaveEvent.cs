using UnityEngine;

public class E_WasteWaveEvent : MonoBehaviour
{
    // Méthode appelée au début de l’événement "Vague de déchets"
    public void OnWasteWaveStart()
    {
        // Réduire le taux de spawn des poissons
        if (E_FishSpawner.Instance != null)
        {
            E_FishSpawner.Instance.ReduceFishSpawnRate();
        }
        // Augmenter le taux de spawn des Trash
        if (E_TrashSpawner.Instance != null)
        {
            E_TrashSpawner.Instance.IncreaseTrashSpawnRate();
        }
        Debug.Log("Vague de déchets démarrée : spawn des poissons réduit, spawn des Trash augmenté.");
    }

    // Méthode appelée à la fin de l’événement "Vague de déchets"
    public void OnWasteWaveEnd()
    {
        // Restaurer le taux de spawn des poissons
        if (E_FishSpawner.Instance != null)
        {
            E_FishSpawner.Instance.RestoreDefaultSpawnRate();
        }
        // Restaurer le taux de spawn des Trash à la valeur par défaut
        if (E_TrashSpawner.Instance != null)
        {
            E_TrashSpawner.Instance.RestoreDefaultTrashSpawnRate();
        }
        Debug.Log("Vague de déchets terminée : spawn restauré pour poissons et Trash.");
    }
}
