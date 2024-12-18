using UnityEngine;
using TMPro;

public class J_DisplayDateTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;

    void Update()
    {
        if (J_TimeManager.Instance != null)
        {
            // Récupérer jour et mois
            int currentDay = J_TimeManager.Instance.currentDay;
            int currentMonth = J_TimeManager.Instance.currentMonth;

            // Calculer l'heure factice
            float secondsPerDay = J_TimeManager.Instance.secondsPerDay;
            float dayTimer = GetPrivateDayTimer();
            // dayTimer sera récupéré en utilisant une petite astuce (voir plus bas) ou on modifie le TimeManager pour l'exposer

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

    // Méthode pour récupérer dayTimer si on le rend public ou via une propriété
    float GetPrivateDayTimer()
    {
        // Idéalement, on modifie J_TimeManager pour exposer dayTimer en readonly :
        // public float CurrentDayTimer => dayTimer; 
        // puis on l'appellerait ici :
        // return J_TimeManager.Instance.CurrentDayTimer;

        // Si on ne veut pas modifier J_TimeManager, on peut l'adapter :
        // Pour l'instant, supposons qu'on a fait un petit changement dans J_TimeManager 
        // pour exposer dayTimer comme une propriété publique en lecture seule.
        return J_TimeManager.Instance.GetCurrentDayTimer();
    }
}
