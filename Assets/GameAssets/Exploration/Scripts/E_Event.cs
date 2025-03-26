using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class E_Event : MonoBehaviour
{
    private bool isEventActive = false;

    // Référence à l’asset de configuration des événements
    public E_EventSettings eventSettings;

    // Référence au bouton d’événement (assigné dans l’inspecteur ou recherché par tag)
    public GameObject eventButton;

    /// <summary>
    /// Déclenche l’événement avec l’ID spécifié et pour la durée donnée (en mois in game).
    /// </summary>
    public void TriggerEvent(int eventID, int durationInMonths)
    {
        if (isEventActive)
            return;
        StartCoroutine(RunEvent(eventID, durationInMonths));
    }

    IEnumerator RunEvent(int eventID, int durationInMonths)
    {
        isEventActive = true;

        // Si le bouton n’est pas assigné, le chercher via son tag (même inactif)
        if (eventButton == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("eventbutton"))
                {
                    eventButton = obj;
                    break;
                }
            }
            if (eventButton == null)
            {
                Debug.LogWarning("Aucun GameObject avec le tag 'eventbutton' n'a été trouvé !");
            }
            else
            {
                Debug.Log("Bouton d'événement trouvé via le tag 'eventbutton' (inclus les objets inactifs).");
            }
        }

        // Activation du bouton d’événement
        if (eventButton != null)
        {
            eventButton.SetActive(true);
            Debug.Log("Bouton d'événement activé.");
        }
        else
        {
            Debug.LogWarning("eventButton n'est pas assigné !");
        }

        Debug.Log("Début de l'événement " + eventID + " pour " + durationInMonths + " mois in game.");

        // Déclenchement du dialogue dans la scène de gestion ("SampleScene")
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            Speech dialogueToUse = null;
            NormalEventType normalEvent = eventSettings.normalEvents.Find(e => e.eventID == eventID);
            if (normalEvent != null && normalEvent.dialogue != null)
            {
                dialogueToUse = normalEvent.dialogue;
                Debug.Log("Dialogue trouvé dans NormalEvents pour l'event " + eventID + ".");
            }
            else
            {
                InvasionType invasionEvent = eventSettings.invasionTypes.Find(e => e.eventID == eventID);
                if (invasionEvent != null && invasionEvent.dialogue != null)
                {
                    dialogueToUse = invasionEvent.dialogue;
                    Debug.Log("Dialogue trouvé dans InvasionTypes pour l'event " + eventID + ".");
                }
            }
            if (dialogueToUse != null)
            {
                if (ShowDialogue.Instance != null)
                {
                    Debug.Log("Lancement du dialogue pour l'événement " + eventID + ".");
                    ShowDialogue.Instance.DialogueBox(dialogueToUse);
                }
                else
                {
                    Debug.LogWarning("ShowDialogue.Instance non trouvé dans la scène.");
                }
            }
            else
            {
                Debug.LogWarning("Aucun dialogue configuré pour l'event " + eventID + ".");
            }
        }

        // Gestion des effets propres à l’événement selon son ID
        // Répartition des IDs (exemple) :
        // 0 : Vague de déchets (négatif – déjà existant)
        // 1 : Marée noire (négatif – déjà existant)
        // 2 : Invasion de méduses (négatif)
        // 3 : Fête du corail (positif, déclenché par CycleEventManager)
        // 4 : Invasion poisson-lion (négatif)
        // 5 : Invasion de barracudas (négatif)
        // 6 : Coupure de courant (négatif)
        // 7 : Surconsommation énergie (négatif)
        // 8 : Grève (négatif)
        // 9 : Pêche illégale (négatif)
        // 10 : Canicule marine (négatif)
        // 11 : Fuite d’eau usée (négatif)
        // 12 : Nouvelle espèce envahissante (négatif)
        // 13 : Perte de rendement (négatif)
        // 14 : Journée de ramassage (positif)
        // 15 : Mois réduction consommation énergie (positif)
        // 16 : Collecte de fonds (positif)
        // 17 : Remerciement des habitants (positif)
        // 18 : Don d’une société (positif)
        // 19 : Retour des posidonies (positif)
        // 20 : Programme de restauration écosystèmes (positif)
        // 21 : Panne d’énergie (système)
        // 22 : Feux de forêt (système)
        // 23 : Marée rouge (système)
        switch (eventID)
        {
            case 0: // Vague de déchets
                if (E_FishSpawner.Instance != null)
                {
                    E_FishSpawner.Instance.ActivateTrashWaveEffect();
                    Debug.Log("TrashWaveEffect activé pour E_FishSpawner.");
                }
                if (E_TrashSpawner.Instance != null)
                {
                    E_TrashSpawner.Instance.ActivateTrashWaveEffect();
                    Debug.Log("TrashWaveEffect activé pour E_TrashSpawner.");
                }
                break;
            case 1: // Marée noire
                {
                    GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
                    if (blackOverlay != null)
                    {
                        SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            Color col = sr.color;
                            col.a = 0.4f;
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
                break;
            case 2: // Invasion de méduses
                {
                    InvasionType jellyfishEvent = eventSettings.invasionTypes.Find(e => e.eventID == 2);
                    if (jellyfishEvent != null && jellyfishEvent.prefabs.Length > 0)
                    {
                        E_FishSpawner.Instance.EnableInvasionMode(jellyfishEvent.prefabs[0]);
                        Debug.Log("Invasion de méduses activée : remplacement des poissons par méduses.");
                    }
                    else
                    {
                        Debug.LogWarning("Prefab pour Invasion de méduses non trouvé !");
                    }
                }
                break;
            case 4: // Invasion poisson-lion
                {
                    InvasionType lionfishEvent = eventSettings.invasionTypes.Find(e => e.eventID == 4);
                    if (lionfishEvent != null && lionfishEvent.prefabs.Length > 0)
                    {
                        E_FishSpawner.Instance.EnableInvasionMode(lionfishEvent.prefabs[0]);
                        Debug.Log("Invasion poisson-lion activée : réduction des F&F, danger accru.");
                    }
                    else
                    {
                        Debug.LogWarning("Prefab pour Invasion poisson-lion non trouvé !");
                    }
                }
                break;
            case 5: // Invasion de barracudas
                {
                    InvasionType barracudaEvent = eventSettings.invasionTypes.Find(e => e.eventID == 5);
                    if (barracudaEvent != null && barracudaEvent.prefabs.Length > 0)
                    {
                        E_FishSpawner.Instance.EnableInvasionMode(barracudaEvent.prefabs[0]);
                        Debug.Log("Invasion de barracudas activée : zone plus dangereuse, F&F remplacés.");
                    }
                    else
                    {
                        Debug.LogWarning("Prefab pour Invasion de barracudas non trouvé !");
                    }
                }
                break;
            case 6: // Coupure de courant
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                    {
                        builder.enabled = false;
                    }
                    Materials.instance.price = Mathf.Max(Materials.instance.price - 50, 0);
                    Debug.Log("Coupure de courant activée : bâtiments en pause, argent réduit.");
                }
                break;
            case 7: // Surconsommation énergie
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                    {
                        builder.cycleDuration *= 1.5f;
                    }
                    Debug.Log("Surconsommation énergie activée : consommation augmentée dans les bâtiments.");
                }
                break;
            case 8: // Grève
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                    {
                        builder.enabled = false;
                    }
                    Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.2f, 0f);
                    Debug.Log("Grève activée : bâtiments en panne, qualité de vie diminuée.");
                }
                break;
            case 9: // Pêche illégale
                {
                    if (E_FishSpawner.Instance != null)
                    {
                        E_FishSpawner.Instance.minSpawnInterval *= 1.5f;
                        E_FishSpawner.Instance.maxSpawnInterval *= 1.5f;
                    }
                    Debug.Log("Pêche illégale activée : spawn de poissons réduit.");
                }
                break;
            case 10: // Canicule marine
                {
                    if (E_FishSpawner.Instance != null)
                    {
                        E_FishSpawner.Instance.minSpawnInterval *= 1.2f;
                        E_FishSpawner.Instance.maxSpawnInterval *= 1.2f;
                    }
                    Debug.Log("Canicule marine activée : spawn de poissons réduit.");
                }
                break;
            case 11: // Fuite d’eau usée
                {
                    Materials.instance.bar_2 = Mathf.Min(Materials.instance.bar_2 + 0.2f, 1f);
                    Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.1f, 0f);
                    Debug.Log("Fuite d’eau usée activée : biodiversité et qualité de vie diminuées.");
                }
                break;
            case 12: // Nouvelle espèce envahissante
                {
                    InvasionType invasiveEvent = eventSettings.invasionTypes.Find(e => e.eventID == 12);
                    if (invasiveEvent != null && invasiveEvent.prefabs.Length > 0)
                    {
                        E_FishSpawner.Instance.EnableInvasionMode(invasiveEvent.prefabs[0]);
                        Debug.Log("Nouvelle espèce envahissante activée : F&F modifiés.");
                    }
                    else
                    {
                        Debug.LogWarning("Prefab pour Nouvelle espèce envahissante non trouvé !");
                    }
                }
                break;
            case 13: // Perte de rendement
                {
                    Materials.instance.price = Mathf.Max(Materials.instance.price - 100, 0);
                    Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.1f, 0f);
                    Debug.Log("Perte de rendement activée : revenu bloqué, qualité de vie diminuée.");
                }
                break;
            case 14: // Journée de ramassage (positif)
                {
                    Materials.instance.bar_2 = Mathf.Max(Materials.instance.bar_2 - 0.2f, 0f);
                    Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 0.99f);
                    Debug.Log("Journée de ramassage activée : pollution réduite, qualité de vie améliorée.");
                }
                break;
            case 15: // Mois réduction consommation énergie (positif)
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                    {
                        builder.cycleDuration *= 0.8f;
                    }
                    Debug.Log("Mois réduction consommation énergie activé : consommation réduite dans les bâtiments.");
                }
                break;
            case 16: // Collecte de fonds (positif)
                {
                    Materials.instance.price += 700;
                    Debug.Log("Collecte de fonds activée : argent augmenté.");
                }
                break;
            case 17: // Remerciement des habitants (positif)
                {
                    Materials.instance.price += 150;
                    Debug.Log("Remerciement des habitants activé : argent augmenté.");
                }
                break;
            case 18: // Don d’une société (positif)
                {
                    Materials.instance.price += 150;
                    Debug.Log("Don d’une société activé : argent augmenté.");
                }
                break;
            case 19: // Retour des posidonies (positif)
                {
                    Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.15f, 0.99f);
                    Materials.instance.price += 50;
                    Debug.Log("Retour des posidonies activé : biodiversité et qualité de vie améliorées, argent augmenté.");
                }
                break;
            case 20: // Programme de restauration écosystèmes (positif)
                {
                    Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 0.99f);
                    Materials.instance.price = Mathf.Max(Materials.instance.price - 50, 0);
                    Debug.Log("Programme de restauration écosystèmes activé : biodiversité et qualité de vie améliorées, argent diminué.");
                }
                break;
            case 21: // Panne d’énergie (système)
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    if (builders.Length > 0)
                    {
                        int randIndex = Random.Range(0, builders.Length);
                        builders[randIndex].enabled = false;
                        Debug.Log("Panne d’énergie activée : bâtiment " + builders[randIndex].name + " en pause temporaire.");
                    }
                }
                break;
            case 22: // Feux de forêt (système)
                {
                    Debug.Log("Feux de forêt activés : impact potentiel sur le tourisme et la biodiversité.");
                }
                break;
            case 23: // Marée rouge (système)
                {
                    Materials.instance.bar_2 = Mathf.Min(Materials.instance.bar_2 + 0.3f, 1f);
                    Debug.Log("Marée rouge activée : biodiversité impactée par une faune toxique.");
                }
                break;
            default:
                Debug.Log("Aucun effet défini pour l'eventID " + eventID);
                break;
        }

        // Attente basée sur le temps in game (durée de l'événement)
        int startYear = J_TimeManager.Instance.currentYear;
        int startMonth = J_TimeManager.Instance.currentMonth;
        int monthsPassed = 0;
        while (monthsPassed < durationInMonths)
        {
            yield return null;
            int currentYear = J_TimeManager.Instance.currentYear;
            int currentMonth = J_TimeManager.Instance.currentMonth;
            monthsPassed = (currentYear - startYear) * 12 + (currentMonth - startMonth);
        }
        Debug.Log("Fin de l'événement " + eventID + ".");

        // Restauration (ou réversion) des effets appliqués
        switch (eventID)
        {
            case 0:
                if (E_FishSpawner.Instance != null)
                    E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                if (E_TrashSpawner.Instance != null)
                    E_TrashSpawner.Instance.RestoreDefaultTrashSpawnRate();
                break;
            case 1:
                if (E_FishSpawner.Instance != null)
                    E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                {
                    GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
                    if (blackOverlay != null)
                    {
                        SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                        if (sr != null)
                        {
                            Color col = sr.color;
                            col.a = 0f;
                            sr.color = col;
                        }
                    }
                }
                break;
            case 2:
            case 4:
            case 5:
            case 12:
                if (E_FishSpawner.Instance != null && E_FishSpawner.Instance.invasionModeActive)
                    E_FishSpawner.Instance.DisableInvasionMode();
                break;
            case 6:
            case 8:
            case 21:
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                        builder.enabled = true;
                }
                break;
            case 7:
                {
                    Builder[] builders = FindObjectsOfType<Builder>();
                    foreach (var builder in builders)
                        builder.cycleDuration /= 1.5f;
                }
                break;
            case 9:
            case 10:
                if (E_FishSpawner.Instance != null)
                    E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                break;
            case 11:
                Materials.instance.bar_2 = 0.6f; // valeur par défaut
                break;
            case 22:
            case 23:
                if (E_FishSpawner.Instance != null)
                    E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                break;
            default:
                break;
        }

        // Désactivation du bouton d’événement à la fin
        if (eventButton != null)
        {
            eventButton.SetActive(false);
            Debug.Log("Bouton d'événement désactivé.");
        }

        isEventActive = false;
        // Notifier le Cycle Event Manager que l'événement est terminé (pour lancer le cooldown)
        E_CycleEventManager cycleManager = FindObjectOfType<E_CycleEventManager>();
        if (cycleManager != null)
        {
            cycleManager.EndEvent();
            Debug.Log("Cycle Event Manager notifié de la fin de l'événement.");
        }
        else
        {
            Debug.LogWarning("E_CycleEventManager non trouvé !");
        }
    }
}
