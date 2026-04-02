using UnityEngine;
using UnityEngine.UI;

public class Recycler : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button[] recycleButtons;

    void Update()
    {
        CheckButtonInteractivity();
    }

    private void CheckButtonInteractivity()
    {
        // Condition : avoir au moins 20 argent et 10 déchets
        bool canAfford = Materials.instance.price >= 20 && Materials.instance.trash >= 10;

        foreach (Button btn in recycleButtons)
        {
            if (btn != null)
            {
                btn.interactable = canAfford;
            }
        }
    }

    public void Pay(int mat)
    {
        if (Materials.instance.price >= 20 && Materials.instance.trash >= 10)
        {
            // --- MISE À JOUR DES RESSOURCES ---
            Materials.instance.price -= 20;
            Materials.instance.trash -= 10;

            // Correction ici : ajout du 'f' pour éviter l'erreur CS0266
            Materials.instance.bar_2 -= 0.05f;

            // --- ATTRIBUTION DU MATÉRIAU ---
            if (mat == 0) Materials.instance.mat_0 += 20;
            else if (mat == 1) Materials.instance.mat_1 += 20;
            else if (mat == 2) Materials.instance.mat_2 += 20;
        }
    }
}