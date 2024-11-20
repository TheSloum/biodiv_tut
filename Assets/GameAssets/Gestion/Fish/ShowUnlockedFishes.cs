using UnityEngine;
using System.Text;
using System.Collections.Generic;

public class ShowUnlockedFishes : MonoBehaviour
{
    /// <summary>
    /// Méthode appelée lorsque le bouton est cliqué.
    /// Affiche dans les logs la liste des poissons déverrouillés.
    /// </summary>
    public void ShowUnlockedFishesInLog()
    {
        if (FishUnlock.Instance == null)
        {
            Debug.LogError("FishUnlock Instance n'est pas trouvé. Assure-toi que FishUnlockManager est présent dans la scène initiale.");
            return;
        }

        // Récupérer la liste des poissons déverrouillés
        List<Fishes> unlockedFishes = FishUnlock.Instance.fishes.FindAll(fish => fish.is_unlocked);

        if (unlockedFishes.Count == 0)
        {
            Debug.Log("Aucun poisson n'est déverrouillé.");
            return;
        }

        // Construire la chaîne de caractères avec les noms des poissons déverrouillés
        StringBuilder sb = new StringBuilder();
        sb.Append("Les poissons débloqués sont : ");

        for (int i = 0; i < unlockedFishes.Count; i++)
        {
            sb.Append(unlockedFishes[i].fishName);
            if (i < unlockedFishes.Count - 1)
            {
                sb.Append(", ");
            }
        }

        // Afficher dans les logs
        Debug.Log(sb.ToString());
    }
}
