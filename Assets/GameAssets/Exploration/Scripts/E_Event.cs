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

        // Gestion des effets propres à l’événement
        if (eventID == 0)
        {
            // Vague de déchets (déjà existant)
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.ActivateTrashWaveEffect();
            }
            if (E_TrashSpawner.Instance != null)
            {
                E_TrashSpawner.Instance.ActivateTrashWaveEffect();
            }
        }
        else if (eventID == 1)
        {
            // Effets pour MareeNoire
            // 1. Modifier l'opacité du BlackOverlay via son SpriteRenderer
            GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
            if (blackOverlay != null)
            {
                SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color col = sr.color;
                    col.a = 0.7f; // 70% d'opacité
                    sr.color = col;
                    Debug.Log("MareeNoire : Opacité du BlackOverlay mise à 70%.");
                }
                else
                {
                    Debug.LogWarning("MareeNoire : Aucun SpriteRenderer trouvé sur BlackOverlay !");
                }
            }
            else
            {
                Debug.LogWarning("MareeNoire : BlackOverlay non trouvé !");
            }
            // 2. Diminuer l'intervalle de spawn (augmenter le taux de spawn)
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.IncreaseFishSpawnRate();
                Debug.Log("MareeNoire : Intervalle de spawn diminué.");
            }
        }
        else if (eventID == 2)
        {
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.ActivateTrashWaveEffect();
            }
        }

        // Calcul de la durée réelle en secondes
        float secondsPerMonth = J_TimeManager.Instance.secondsPerDay * J_TimeManager.Instance.daysPerMonth;
        float realDuration = durationInMonths * secondsPerMonth;
        yield return new WaitForSeconds(realDuration);

        Debug.Log("Fin de l'événement " + eventID + ".");

        // Fin des effets spécifiques à l’événement
        if (eventID == 0)
        {
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.RestoreDefaultSpawnRate();
            }
            if (E_TrashSpawner.Instance != null)
            {
                E_TrashSpawner.Instance.RestoreDefaultTrashSpawnRate();
            }
        }
        else if (eventID == 1)
        {
            // Restaurer l'intervalle de spawn par défaut
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                Debug.Log("MareeNoire terminé : Intervalle de spawn restauré.");
            }
            // Réinitialiser l'opacité du BlackOverlay à 0%
            GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
            if (blackOverlay != null)
            {
                SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color col = sr.color;
                    col.a = 0f; // Opacité 0%
                    sr.color = col;
                    Debug.Log("MareeNoire : Opacité du BlackOverlay remise à 0%.");
                }
                else
                {
                    Debug.LogWarning("MareeNoire : Aucun SpriteRenderer trouvé sur BlackOverlay !");
                }
            }
            else
            {
                Debug.LogWarning("MareeNoire : BlackOverlay non trouvé !");
            }
        }
        else if (eventID == 2)
        {
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.RestoreDefaultSpawnRate();
            }
        }

        // Désactivation du mode invasion si actif
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
