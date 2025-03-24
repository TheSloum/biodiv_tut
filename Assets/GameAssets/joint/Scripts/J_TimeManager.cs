using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class J_TimeManager : MonoBehaviour
{
    public static J_TimeManager Instance;

    [Header("Debug Settings")]
    [SerializeField] private bool enableTimeAcceleration = true;
    [SerializeField] private float debugTimeMultiplier = 1f;
    [SerializeField] private float debugTimeAcceleration = 2f;
    [SerializeField] private float maxTimeMultiplier = 10f;

    [Header("Time Settings")]
    [Tooltip("Durée d'un jour en secondes réelles.")]
    public float secondsPerDay = 10f;
    [Tooltip("Nombre de jours par mois.")]
    public int daysPerMonth = 30;

    [Header("Current Time")]
    [Tooltip("Jour actuel (commence à 1).")]
    public int currentDay = 1;
    [Tooltip("Mois actuel (commence à 1).")]
    public int currentMonth = 1;
    [Tooltip("Année actuelle (commence à 1).")]
    public int currentYear = 1;

    private float dayTimer = 0f;
    private bool initialized = false;

    public event Action<int, int> OnDayChanged;
    public event Action<int> OnMonthChanged;
    public event Action<int> OnYearChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Détache l'objet de tout parent pour éviter la destruction avec un parent
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Debug.LogWarning("Duplicate J_TimeManager destroyed: " + gameObject.name);
            Destroy(gameObject);
        }
    }
    public void ResetState()
    {
        // Réinitialiser les paramètres de temps
        currentDay = 1;
        currentMonth = 1;
        currentYear = 1;
        dayTimer = 0f;
        initialized = false;

        // Réinitialiser les paramètres de débogage
        debugTimeMultiplier = 1f;
        Time.timeScale = 1f;

        // Déclencher les événements pour mettre à jour l'UI si nécessaire
        OnDayChanged?.Invoke(currentDay, currentMonth);
        OnMonthChanged?.Invoke(currentMonth);
        OnYearChanged?.Invoke(currentYear);

        Debug.Log("✅ J_TimeManager a été réinitialisé !");
    }

    void Start()
    {
        initialized = true;
    }

    void Update()
    {
        HandleDebugTimeInput();
        if (!initialized) return;

        dayTimer += Time.deltaTime;

        if (dayTimer >= secondsPerDay)
        {
            dayTimer -= secondsPerDay;
            IncrementDay();
        }
    }

    private void HandleDebugTimeInput()
    {
        if (!enableTimeAcceleration) return;

        if (Input.GetKey(KeyCode.T))
        {
            debugTimeMultiplier = Mathf.Clamp(
                debugTimeMultiplier + debugTimeAcceleration * Time.unscaledDeltaTime,
                1f,
                maxTimeMultiplier
            );

            Time.timeScale = debugTimeMultiplier;
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            debugTimeMultiplier = 1f;
            Time.timeScale = 1f;
        }
    }

    private void IncrementDay()
    {
        currentDay++;

        if (currentDay > daysPerMonth)
        {
            currentDay = 1;
            currentMonth++;

            if (currentMonth > 12)
            {
                currentMonth = 1;
                currentYear++;
                OnYearChanged?.Invoke(currentYear);
            }

            OnMonthChanged?.Invoke(currentMonth);
        }
        OnDayChanged?.Invoke(currentDay, currentMonth);
    }

    public void SetTime(int day, int month, int year, bool resetTimer = true)
    {
        currentDay = day;
        currentMonth = month;
        currentYear = year;
        initialized = true;
    }

    public string GetFormattedDate()
    {
        return $"Mois {currentMonth}, Année {currentYear}";
    }

    public void SetTimeSpeed(float speed) => Time.timeScale = speed;
    public void PauseTime() => Time.timeScale = 0f;
    public void ResumeTime() => Time.timeScale = 1f;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public int GetCurrentYear() => currentYear;
    public int GetCurrentMonth() => currentMonth;
    public float GetCurrentTimer() => dayTimer;
}