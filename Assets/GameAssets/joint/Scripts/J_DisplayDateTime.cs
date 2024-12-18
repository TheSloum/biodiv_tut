using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class J_DisplayDateTime : MonoBehaviour
{
    private TextMeshProUGUI dateText;

    void Awake()
    {
        FindDateText();
    }

    void OnEnable()
    {
        // S'abonner à l'événement de chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Se désabonner de l'événement pour éviter les erreurs
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (dateText == null) return;  // Si dateText n'est pas trouvé, on ne fait rien

        if (J_TimeManager.Instance != null)
        {
            int currentDay = J_TimeManager.Instance.currentDay;
            int currentMonth = J_TimeManager.Instance.currentMonth;

            float secondsPerDay = J_TimeManager.Instance.secondsPerDay;
            float dayTimer = GetPrivateDayTimer();

            float currentDayProgress = dayTimer / secondsPerDay;
            float currentHour = currentDayProgress * 24f;
            int hourInt = Mathf.FloorToInt(currentHour);

            dateText.text = $"Jour {currentDay}, Mois {currentMonth} - {hourInt}h";
        }
        else
        {
            dateText.text = "Time Manager non trouvé.";
        }
    }

    float GetPrivateDayTimer()
    {
        return J_TimeManager.Instance.GetCurrentDayTimer();
    }

    private void FindDateText()
    {
        GameObject dateTextObject = GameObject.FindGameObjectWithTag("DateText");
        if (dateTextObject != null)
        {
            dateText = dateTextObject.GetComponent<TextMeshProUGUI>();
            if (dateText == null)
            {
                Debug.LogWarning("Le GameObject avec le tag 'DateText' n'a pas de composant TextMeshProUGUI.");
            }
        }
        else
        {
            Debug.LogWarning("Aucun GameObject avec le tag 'DateText' trouvé dans la scène.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Quand une nouvelle scène est chargée, on recherche à nouveau le texte
        FindDateText();
    }
}
