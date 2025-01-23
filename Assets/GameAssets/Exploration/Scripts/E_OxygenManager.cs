using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class E_OxygenManager : MonoBehaviour
{
    public Slider oxygenSlider;
    public float currentOxygen;
    public float maxOxygen = 100f;

    [SerializeField] private float depletionRate = 5f; 
    [SerializeField] private TextMeshProUGUI trashCounterText; 
    [SerializeField] private GameObject resultScreen;
    [SerializeField] private TextMeshProUGUI timeText; 
    [SerializeField] private TextMeshProUGUI woodText; 
    [SerializeField] private TextMeshProUGUI stoneText; 
    [SerializeField] private TextMeshProUGUI ironText; 
    [SerializeField] private GameObject gameOverCanvas;

    [HideInInspector] public int trashCollected = 0;

    private float startTime;
    private bool isGameOver = false;

    void Start()
    {
        // CHANGEMENT : Réinitialisation des valeurs au start
        currentOxygen = maxOxygen;
        oxygenSlider.maxValue = maxOxygen;
        oxygenSlider.value = currentOxygen;

        trashCollected = 0;
        UpdateTrashCounterUI();

        if (resultScreen != null)
        {
            resultScreen.SetActive(false);
        }

        if (Materials.instance != null)
        {
            Materials.instance.ResetSessionCounts();
        }

        isGameOver = false;
        startTime = Time.time;
    }

    void Update()
    {
        if (currentOxygen > 0 && !isGameOver)
        {
            currentOxygen -= depletionRate * Time.deltaTime;
            currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
            oxygenSlider.value = currentOxygen;

            if (currentOxygen <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public void AddOxygen(float amount)
    {
        currentOxygen += amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        oxygenSlider.value = currentOxygen;
    }

    public void DecreaseOxygen(float amount)
    {
        currentOxygen -= amount;
        currentOxygen = Mathf.Clamp(currentOxygen, 0, maxOxygen);
        oxygenSlider.value = currentOxygen;

        if (currentOxygen <= 0)
        {
            TriggerGameOver();
        }
    }

    public void IncrementTrashCollected()
    {
        trashCollected++;
        UpdateTrashCounterUI();
    }

    public void UpdateTrashCounterUI()
    {
        if (trashCounterText != null)
        {
            trashCounterText.text = "Trash Collectés : " + trashCollected.ToString();
        }
    }

    void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("Oxygène épuisé !");
        float totalTime = Time.time - startTime;
        string formattedTime = FormatTime(totalTime);

        if (resultScreen != null)
        {
            resultScreen.SetActive(true);
            StartCoroutine(DisplayResults(formattedTime));
        }

        if(gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    IEnumerator DisplayResults(string formattedTime)
    {
        if (timeText != null)
        {
            timeText.text = "Temps Total : 0:00";
            yield return StartCoroutine(AnimateText(timeText, "Temps Total : ", formattedTime, 2f));
        }

        yield return StartCoroutine(ShowResourceCount(woodText, "Bois Collecté : ", Materials.instance != null ? Materials.instance.sessionWood : 0, 1.5f));
        yield return StartCoroutine(ShowResourceCount(stoneText, "Pierre Collectée : ", Materials.instance != null ? Materials.instance.sessionStone : 0, 1.5f));
        yield return StartCoroutine(ShowResourceCount(ironText, "Fer Collecté : ", Materials.instance != null ? Materials.instance.sessionIron : 0, 1.5f));

        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;
        Materials.instance.explored = true;
        Materials.instance.isLoad = true;
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator ShowResourceCount(TextMeshProUGUI textComponent, string prefix, int targetValue, float duration)
    {
        if (textComponent != null && Materials.instance != null)
        {
            textComponent.text = prefix + "0";
            yield return StartCoroutine(AnimateNumberText(textComponent, prefix, targetValue, duration));
        }
        else if (textComponent != null)
        {
            textComponent.text = prefix + targetValue.ToString();
            Debug.LogWarning("Materials.instance est null ou " + textComponent.name + " est non assigné dans DisplayResults()");
        }
    }

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
            if (Input.GetKeyDown(KeyCode.Return))
            {
                currentMinutes = targetMinutes;
                currentSeconds = targetSeconds;
                textComponent.text = prefix + targetTime;
                yield break;
            }

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            currentMinutes = Mathf.FloorToInt(Mathf.Lerp(0, targetMinutes, t));
            currentSeconds = Mathf.FloorToInt(Mathf.Lerp(0, targetSeconds, t));
            if (currentSeconds > 59)
            {
                currentSeconds = 59;
            }

            textComponent.text = prefix + currentMinutes.ToString("0") + ":" + currentSeconds.ToString("00");
            yield return null;
        }

        textComponent.text = prefix + targetTime;
    }

    IEnumerator AnimateNumberText(TextMeshProUGUI textComponent, string prefix, int targetNumber, float duration)
    {
        float elapsed = 0f;
        int currentNumber = 0;

        while (elapsed < duration)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                textComponent.text = prefix + targetNumber.ToString();
                yield break;
            }

            elapsed += Time.unscaledDeltaTime; 
            float t = Mathf.Clamp01(elapsed / duration);

            currentNumber = Mathf.FloorToInt(Mathf.Lerp(0, targetNumber, t));
            textComponent.text = prefix + currentNumber.ToString();
            yield return null;
        }

        textComponent.text = prefix + targetNumber.ToString();
    }

    string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f;
        Materials.instance.explored = true;
        Materials.instance.isLoad = true;
        SceneManager.LoadScene("SampleScene");
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("Quitter le jeu.");
    }

    // CHANGEMENT : Méthode pour réinitialiser l'oxygène, appelée après chargement de scène
    public void ResetOxygen()
    {
        currentOxygen = maxOxygen;
        oxygenSlider.value = currentOxygen;
        trashCollected = 0;
        UpdateTrashCounterUI();
        isGameOver = false;
        if (resultScreen != null) resultScreen.SetActive(false);
        if (Materials.instance != null) Materials.instance.ResetSessionCounts();
    }
}
