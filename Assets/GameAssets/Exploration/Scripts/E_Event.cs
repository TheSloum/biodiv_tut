using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Pour vérifier le nom de la scène

public class E_Event : MonoBehaviour
{
    private bool isEventActive = false;

    // Référence à l'asset de configuration des événements
    public E_EventSettings eventSettings;

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

        // --- Déclenchement du dialogue uniquement dans la scène de gestion ("SampleScene") ---
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            Speech dialogueToUse = null;
            // Recherche dans les NormalEvents
            NormalEventType normalEvent = eventSettings.normalEvents.Find(e => e.eventID == eventID);
            if (normalEvent != null && normalEvent.dialogue != null)
            {
                dialogueToUse = normalEvent.dialogue;
            }
            else
            {
                // Recherche dans les InvasionTypes
                InvasionType invasionEvent = eventSettings.invasionTypes.Find(e => e.eventID == eventID);
                if (invasionEvent != null && invasionEvent.dialogue != null)
                {
                    dialogueToUse = invasionEvent.dialogue;
                }
            }

            if (dialogueToUse != null)
            {
                if (ShowDialogue.Instance != null)
                {
                    ShowDialogue.Instance.DialogueBox(dialogueToUse);
                }
                else
                {
                    Debug.LogWarning("ShowDialogue.Instance non trouvé dans la scène.");
                }
            }
        }
        // --- Fin du déclenchement du dialogue ---

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
            GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
            if (blackOverlay != null)
            {
                SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color col = sr.color;
                    col.a = 0.4f; // 40% d'opacité
                    sr.color = col;
                    Debug.Log("MareeNoire : Opacité du BlackOverlay mise à 40%.");
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
        else if (eventID == 3)
        {
            // --- Début de l'Event 3 ---
            Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.2f, 0f);
            Materials.instance.event3Active = true;
            Debug.Log("Event 3 activé : Qualité de vie diminuée.");

            // Augmenter le temps des cycles pour les bâtiments énergie/tourisme.
            Builder[] builders = FindObjectsOfType<Builder>();
            foreach (var builder in builders)
            {
                if (builder.buildClass == 1 || builder.buildClass == 2)
                {
                    builder.cycleDuration *= 1.5f; // Augmente de 50%
                    Debug.Log("Cycle augmenté pour un bâtiment de type " + builder.buildClass);
                }
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
            if (E_FishSpawner.Instance != null)
            {
                E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                Debug.Log("MareeNoire terminé : Intervalle de spawn restauré.");
            }
            GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
            if (blackOverlay != null)
            {
                SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color col = sr.color;
                    col.a = 0f;
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
        else if (eventID == 3)
        {
            // --- Fin de l'Event 3 ---
            Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.2f, 0.99f);
            Materials.instance.event3Active = false;
            Debug.Log("Event 3 terminé : Qualité de vie restaurée.");

            Builder[] builders = FindObjectsOfType<Builder>();
            foreach (var builder in builders)
            {
                if (builder.buildClass == 1 || builder.buildClass == 2)
                {
                    builder.cycleDuration /= 1.5f; // Restaure la durée d'origine
                    Debug.Log("Cycle restauré pour un bâtiment de type " + builder.buildClass);
                }
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
