using UnityEngine;
using TMPro;

public class E_MaterialsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI stoneText;
    [SerializeField] private TextMeshProUGUI ironText;

    void Update()
    {
        // Vérifier que l'instance du Materials est bien présente
        if (Materials.instance != null)
        {
            if (woodText != null)
                woodText.text = $"{Materials.instance.sessionWood}";
            if (stoneText != null)
                stoneText.text = $"{Materials.instance.sessionStone}";
            if (ironText != null)
                ironText.text = $"{Materials.instance.sessionIron}";
        }
    }
}
