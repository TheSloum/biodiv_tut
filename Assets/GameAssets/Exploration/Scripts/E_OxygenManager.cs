using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

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

    // UI ResultScreen
    public GameObject resultScreen; // Assignez via l'inspecteur
    public TextMeshProUGUI timeText; // Assignez via l'inspecteur
    public TextMeshProUGUI woodText; // Assignez via l'inspecteur
    public TextMeshProUGUI stoneText; // Assignez via l'inspecteur
    public TextMeshProUGUI ironText; // Assignez via l'inspecteur

    // Variables pour le temps
    private float startTime;
    private bool isGameOver = false;

    public GameObject gameOverCanvas; // Assigné via l'inspecteur

    void Start()
    {
        currentOxygen = maxOxygen;
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = currentOxygen;

        UpdateTrashCounterUI();

        // Initialiser le temps de départ
        startTime = Time.time;

        // Désactiver le ResultScreen au début
        if (resultScreen != null)
        {
            resultScreen.SetActive(false);
        }

        // Réinitialiser les compteurs de session
        if (Materials.instance != null)
        {
            //Materials.instance.ResetSessionCounts();
        }
        else
        {
            Debug.LogWarning("Materials.instance est null dans E_OxygenManager.Start()");
        }
    }

    void Update()
    {
        if (currentOxygen > 0 && !isGameOver)
        {
            // Décrémenter l'oxygène
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            oxygenSlider.value = currentOxygen;

            // Vérifier si le joueur est à court d'oxygène
            if (currentOxygen <= 0)
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
        if (currentOxygen <= 0)
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
        if (isGameOver) return; // Empêcher de déclencher plusieurs fois
        isGameOver = true;

        Debug.Log("Oxygène épuisé !");

        // Calculer le temps total passé en mode exploration
        float totalTime = Time.time - startTime;
        string formattedTime = FormatTime(totalTime);

        // Activer le ResultScreen
        if (resultScreen != null)
        {
            resultScreen.SetActive(true);
            //StartCoroutine(DisplayResults(formattedTime));
        }

        // Activer le GameOverCanvas si nécessaire
        if(gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        // Optionnel : Arrêter le jeu ou désactiver les contrôles du joueur
        Time.timeScale = 0f; // Mettre le jeu en pause
    }

    // Coroutine pour afficher les résultats avec animation
    /*
    
    
    
    
    
    IEnumerator DisplayResults(string formattedTime)
    {
        // Afficher et animer le temps total
        if (timeText != null)
        {
            timeText.text = "Temps Total : 0:00";
            yield return StartCoroutine(AnimateText(timeText, "Temps Total : ", formattedTime, 2f));
        }

        // Afficher et animer le bois collecté
        if (woodText != null && Materials.instance != null)
        {
            woodText.text = "Bois Collecté : 0";
            yield return StartCoroutine(AnimateNumberText(woodText, "Bois Collecté : ", Materials.instance.sessionWood, 1.5f));
        }
        else
        {
            Debug.LogWarning("Materials.instance est null ou woodText est non assigné dans DisplayResults()");
        }

        // Afficher et animer la pierre collectée
        if (stoneText != null && Materials.instance != null)
        {
            stoneText.text = "Pierre Collectée : 0";
            yield return StartCoroutine(AnimateNumberText(stoneText, "Pierre Collectée : ", Materials.instance.sessionStone, 1.5f));
        }
        else
        {
            Debug.LogWarning("Materials.instance est null ou stoneText est non assigné dans DisplayResults()");
        }

        // Afficher et animer le fer collecté
        if (ironText != null && Materials.instance != null)
        {
            ironText.text = "Fer Collecté : 0";
            yield return StartCoroutine(AnimateNumberText(ironText, "Fer Collecté : ", Materials.instance.sessionIron, 1.5f));
        }
        else
        {
            Debug.LogWarning("Materials.instance est null ou ironText est non assigné dans DisplayResults()");
        }

        // Attendre 5 secondes avant de changer de scène
        yield return new WaitForSecondsRealtime(2f);

        // Réinitialiser le temps et la pause
        Time.timeScale = 1f;

        // Charger la scène d'exploration ou la scène principale
        SceneManager.LoadScene("SampleScene"); // Remplacez "SampleScene" par le nom de votre scène d'exploration
    }

    // Coroutine pour animer le texte du temps total
    IEnumerator AnimateText(TextMeshProUGUI textComponent, string prefix, string targetTime, float duration)
    {
        float elapsed = 0f;
        string[] timeParts = targetTime.Split(':');
        int targetMinutes = int.Parse(timeParts[0]);
        int targetSeconds = int.Parse(timeParts[1]);

        int currentMinutes = 0;
        int currentSeconds = 0;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Utiliser undeltaTime non affecté par Time.timeScale
            float t = Mathf.Clamp01(elapsed / duration);

            // Interpoler les minutes et secondes séparément
            currentMinutes = Mathf.FloorToInt(Mathf.Lerp(0, targetMinutes, t));
            currentSeconds = Mathf.FloorToInt(Mathf.Lerp(0, targetSeconds, t));

            // Assurer que les secondes ne dépassent pas 59
            if (currentSeconds > 59)
            {
                currentSeconds = 59;
            }

            textComponent.text = prefix + currentMinutes.ToString("0") + ":" + currentSeconds.ToString("00");

            yield return null;
        }

        // Assurer que le texte final est exact
        textComponent.text = prefix + targetTime;
    }

    // Coroutine pour animer les textes de nombre de matériaux collectés
    IEnumerator AnimateNumberText(TextMeshProUGUI textComponent, string prefix, int targetNumber, float duration)
    {
        float elapsed = 0f;
        int currentNumber = 0;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Utiliser undeltaTime non affecté par Time.timeScale
            float t = Mathf.Clamp01(elapsed / duration);

            // Interpoler le nombre
            currentNumber = Mathf.FloorToInt(Mathf.Lerp(0, targetNumber, t));

            textComponent.text = prefix + currentNumber.ToString();

            yield return null;
        }

        // Assurer que le texte final est exact
        textComponent.text = prefix + targetNumber.ToString();
    } 
    
    
    
    
    
    
    
    
    */

    // Méthode pour formater le temps en minutes:secondes
    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    // Méthode pour recommencer ou quitter après Game Over
    public void OnRestartButtonPressed()
    {
        // Réinitialiser le temps
        Time.timeScale = 1f;

        // Charger la scène d'exploration ou la scène principale
        SceneManager.LoadScene("SampleScene");
    }

    public void OnQuitButtonPressed()
    {
        // Quitter le jeu
        Application.Quit();
        Debug.Log("Quitter le jeu.");
    }
}
