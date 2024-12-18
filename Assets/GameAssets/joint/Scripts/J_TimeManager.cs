using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class J_TimeManager : MonoBehaviour
{
    public static J_TimeManager Instance;

    [Header("Time Settings")]
    [Tooltip("Durée d'un jour en secondes réelles (sans accélération).")]
    public float secondsPerDay = 120f; // Par défaut
    [Tooltip("Nombre de jours par mois.")]
    public int daysPerMonth = 30;

    [Header("Current Time")]
    [Tooltip("Jour actuel (commence à 1).")]
    public int currentDay = 1;
    [Tooltip("Mois actuel (commence à 1).")]
    public int currentMonth = 1;

    private float dayTimer = 0f; 
    private bool initialized = false;

    // Événements
    public event Action<int, int> OnDayChanged;   // (jour, mois)
    public event Action<int> OnMonthChanged;      // (mois)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // S'abonner à l'événement de chargement de scène
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        dayTimer += Time.deltaTime;

        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            IncrementDay();
        }
    }

    private void IncrementDay()
    {
        currentDay++;
        if (currentDay > daysPerMonth)
        {
            currentDay = 1;
            currentMonth++;
            OnMonthChanged?.Invoke(currentMonth);
        }
        OnDayChanged?.Invoke(currentDay, currentMonth);
    }

    /// <summary>
    /// Appelée lorsqu'on veut changer la date sans remettre le timer à zéro.
    /// Passer resetTimer = false pour conserver le temps actuel de la journée.
    /// </summary>
    public void SetTime(int day, int month, bool resetTimer = true)
    {
        currentDay = day;
        currentMonth = month;
        initialized = true;
    }

    public string GetFormattedDate()
    {
        return $"Jour {currentDay}, Mois {currentMonth}";
    }

    public void SetTimeSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public float GetCurrentDayTimer()
    {
        return dayTimer;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // On ajuste seulement la vitesse du temps (secondsPerDay)
        // Sans réinitialiser dayTimer ni appeler SetTime()
        if(scene.name == "Exploration_main")
        {
            // Par exemple, la scène d'exploration a un jour plus long
            secondsPerDay = 90f; 
        }
        else if(scene.name == "SampleScene")
        {
            // Dans une autre scène, le jour est plus court
            secondsPerDay = 10f; 
        }
        // Ne pas toucher à dayTimer ici, ni à currentDay/currentMonth.
        // Le temps continue de s'écouler sans être réinitialisé.
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
