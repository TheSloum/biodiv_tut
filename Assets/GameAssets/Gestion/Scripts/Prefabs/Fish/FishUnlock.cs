using System.Collections.Generic;
using UnityEngine;

public class FishUnlock : MonoBehaviour
{
    public static FishUnlock Instance; // Singleton Instance

    [Header("Liste des Poissons à Déverrouiller")]
    public List<Fishes> fishes;

    private void Awake()
    {
        // Implémenter le Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionnel : persiste entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Tente de déverrouiller un poisson avec l'ID spécifié.
    /// </summary>
    /// <param name="fishID">L'ID du poisson à déverrouiller.</param>
    /// <returns>True si le poisson a été déverrouillé maintenant, False s'il était déjà déverrouillé ou non trouvé.</returns>
    public bool UnlockFish(int fishID)
    {
        foreach (Fishes fish in fishes)
        {
            if (fish.fishID == fishID)
            {
                if (!fish.is_unlocked)
                {
                    fish.UnlockFish();
                    Debug.Log($"Poisson '{fish.fishName}' avec ID {fishID} a été déverrouillé !");
                    return true; // Déverrouillage réussi
                }
                else
                {
                    Debug.Log($"Poisson avec ID {fishID} est déjà déverrouillé.");
                    return false; // Déjà déverrouillé
                }
            }
        }
        Debug.LogWarning($"Poisson avec ID {fishID} non trouvé dans le Pokedex.");
        return false; // Poisson non trouvé
    }
}
