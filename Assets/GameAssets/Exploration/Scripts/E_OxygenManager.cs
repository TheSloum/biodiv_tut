using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class E_OxygenManager : MonoBehaviour
{
    public Slider oxygenSlider;
    public float maxOxygen = 100f;
    public float currentOxygen;
    public float depletionRate = 5f; // oxygène par seconde

    // Variables pour le compteur de Trash
    public TextMeshProUGUI trashCounterText; // Assignez ce champ via l'inspecteur
    [HideInInspector]
    public int trashCollected = 0; // Doit être public pour être accessible

    public GameObject gameOverCanvas; // Assigné via l'inspecteur

    void Start()
    {
        currentOxygen = maxOxygen;
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = currentOxygen;

        UpdateTrashCounterUI();
    }

    void Update()
    {
        if (currentOxygen > 0)
        {
            // Décrémenter l'oxygène
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            oxygenSlider.value = currentOxygen;

            // Vérifier si le joueur est à court d'oxygène
            if(currentOxygen <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    // Méthode pour ajouter de l'oxygène
    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        oxygenSlider.value = currentOxygen;
    }

    // Méthode pour diminuer de l'oxygène
    public void DecreaseOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        oxygenSlider.value = currentOxygen;

        // Vérifier si l'oxygène est épuisé
        if(currentOxygen <= 0)
        {
            TriggerGameOver();
        }
    }

    // Méthode pour incrémenter le compteur de Trash
    public void IncrementTrashCollected()
    {
        trashCollected++;
        UpdateTrashCounterUI();
    }

    // Mettre à jour le texte du compteur de Trash
    public void UpdateTrashCounterUI()
    {
        if(trashCounterText != null)
        {
            trashCounterText.text = "Trash Collectés : " + trashCollected.ToString();
        }
    }

    // Méthode pour déclencher le Game Over
    void TriggerGameOver()
    {
        Debug.Log("Oxygène épuisé !");
        if(gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        // Optionnel : Arrêter le jeu ou désactiver les contrôles du joueur
        Time.timeScale = 0f; // Mettre le jeu en pause
    }
}
