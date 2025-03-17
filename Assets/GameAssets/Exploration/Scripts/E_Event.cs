using System.Collections;
using UnityEngine;

public class E_Event : MonoBehaviour
{
    private bool isEventActive = false;

    /// <summary>
    /// Déclenche l’événement avec l’ID spécifié et pour la durée donnée (en mois in game).
    /// </summary>
    /// <param name="eventID">Identifiant de l’événement</param>
    /// <param name="durationInMonths">Durée de l’événement en mois</param>
    public void TriggerEvent(int eventID, int durationInMonths)
    {
        if (isEventActive)
            return;
        StartCoroutine(RunEvent(eventID, durationInMonths));
    }

    IEnumerator RunEvent(int eventID, int durationInMonths)
    {
        isEventActive = true;
        Debug.Log("Début de l'événement " + eventID + " pour " + durationInMonths + " mois.");

        // Conversion de la durée en mois en secondes réelles via le TimeManager
        float secondsPerMonth = J_TimeManager.Instance.secondsPerDay * J_TimeManager.Instance.daysPerMonth;
        float realDuration = durationInMonths * secondsPerMonth;
        yield return new WaitForSeconds(realDuration);

        Debug.Log("Fin de l'événement " + eventID + ".");

        // Si le mode invasion est actif, le désactiver pour rétablir le spawn normal
        if (E_FishSpawner.Instance != null && E_FishSpawner.Instance.invasionModeActive)
        {
            E_FishSpawner.Instance.DisableInvasionMode();
        }

        isEventActive = false;
        // Notifier le Cycle Event Manager que l'événement est terminé pour lancer le cooldown
        E_CycleEventManager cycleManager = FindObjectOfType<E_CycleEventManager>();
        if (cycleManager != null)
        {
            cycleManager.EndEvent();
        }
    }
}
