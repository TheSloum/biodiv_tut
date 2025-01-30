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
    public float secondsPerDay = 10f; // 1 jour = 10s (pour 30 jours/mois = 5 minutes)
    [Tooltip("Nombre de jours par mois.")]
    public int daysPerMonth = 30; // Rétabli pour afficher les jours

    [Header("Current Time")]
    [Tooltip("Jour actuel (commence à 1).")]
    public int currentDay = 1;
    [Tooltip("Mois actuel (commence à 1).")]
    public int currentMonth = 1;
    [Tooltip("Année actuelle (commence à 1).")]
    public int currentYear = 1; // Nouveau

    private float dayTimer = 0f;
    private bool initialized = false;

    // Événements
    public event Action<int, int> OnDayChanged;   // (jour, mois)
    public event Action<int> OnMonthChanged;      // (mois)
    public event Action<int> OnYearChanged;       // Nouveau (année)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
        if(!enableTimeAcceleration) return;

        if(Input.GetKey(KeyCode.T))
        {
            // Augmente progressivement le multiplicateur
            debugTimeMultiplier = Mathf.Clamp(
                debugTimeMultiplier + debugTimeAcceleration * Time.unscaledDeltaTime,
                1f, 
                maxTimeMultiplier
            );
            
            Time.timeScale = debugTimeMultiplier;
        }
        else if(Input.GetKeyUp(KeyCode.T))
        {
            // Réinitialise le temps quand on relâche la touche
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
            
            // Gestion des années
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

    // Reste inchangé (SetTimeSpeed/PauseTime/ResumeTime)
    public void SetTimeSpeed(float speed) => Time.timeScale = speed;
    public void PauseTime() => Time.timeScale = 0f;
    public void ResumeTime() => Time.timeScale = 1f;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Désactivé pour garder le même temps partout
        // secondsPerDay = 300f; // Forcer la valeur standard
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Nouveaux getters pour l'accès externe
    public int GetCurrentYear() => currentYear;
    public int GetCurrentMonth() => currentMonth;
    public float GetCurrentTimer() => dayTimer;
}