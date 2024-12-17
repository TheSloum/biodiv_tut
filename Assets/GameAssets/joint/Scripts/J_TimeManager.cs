using System;
using UnityEngine;

public class J_TimeManager : MonoBehaviour
{
    public static J_TimeManager Instance;

    [Header("Time Settings")]
    [Tooltip("Durée d'un jour en secondes réelles (sans accélération).")]
    public float secondsPerDay = 120f; // 2 minutes par jour
    [Tooltip("Nombre de jours par mois.")]
    public int daysPerMonth = 30;

    [Header("Current Time")]
    [Tooltip("Jour actuel (commence à 1).")]
    public int currentDay = 1;
    [Tooltip("Mois actuel (commence à 1).")]
    public int currentMonth = 1;

    private float dayTimer = 0f; // Compteur interne du jour en cours, en secondes
    private bool initialized = false;

    // Événements déclenchés à chaque nouveau jour et nouveau mois
    public event Action<int, int> OnDayChanged;   // Paramètres : (jour, mois)
    public event Action<int> OnMonthChanged;      // Paramètre : (mois actuel)

    private void Awake()
    {
        // Petit Singleton des familles
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initialisation. Si on charge une partie, on appellera SetTime() depuis E_GameManager après le chargement.
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        // Avancement du temps du jour
        dayTimer += Time.deltaTime;

        if (dayTimer >= secondsPerDay)
        {
            // On a dépassé la durée d'une journée
            dayTimer -= secondsPerDay; // Conserver l'excédent, au cas où
            IncrementDay();
        }
    }



    void IncrementDay()
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
    /// Méthode appelée lors du chargement de partie pour définir l'état du temps.
    /// </summary>
    public void SetTime(int day, int month)
    {
        currentDay = day;
        currentMonth = month;
        dayTimer = 0f;
        initialized = true;
    }

    /// <summary>
    /// Obtenir une chaîne formatée de la date actuelle (jour, mois).
    /// </summary>
    public string GetFormattedDate()
    {
        return $"Jour {currentDay}, Mois {currentMonth}";
    }

    /// <summary>
    /// Régler la vitesse du temps. Par ex. 1f = normal, 4f = accéléré.
    /// </summary>
    public void SetTimeSpeed(float speed)
    {
        Time.timeScale = speed;
    }

    /// <summary>
    /// Mettre en pause le temps.
    /// </summary>
    public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// Reprendre le temps à vitesse normale.
    /// </summary>
    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public float GetCurrentDayTimer()
    {
        return dayTimer;
    }

}
