using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class E_Event : MonoBehaviour
{
    public static int activeEventID = -1;
    private bool isEventActive = false;

    public E_EventSettings eventSettings;
    public GameObject eventButton;

    public int coralFestivalMonth = 5;
    public int coralFestivalCycle = 2;

    public void TriggerEvent(int eventID, int durationInMonths)
    {
        if (eventID == 3 && !CanTriggerCoralFestival())
        {
            Debug.Log("La Fête du Corail ne peut être déclenchée qu'en " + coralFestivalMonth + " tous les " + coralFestivalCycle + " ans.");
            return;
        }

        if (isEventActive) return;
        StartCoroutine(RunEvent(eventID, durationInMonths));
    }

    private bool CanTriggerCoralFestival()
    {
        int currentYear = J_TimeManager.Instance.currentYear;

        bool isRightYear = (currentYear > 0 && currentYear % coralFestivalCycle == 0);

        Debug.Log($"Check Festival -> Année: {currentYear} | Cycle: {coralFestivalCycle} | Autorisé: {isRightYear}");

        return isRightYear;
    }
    private void OnEnable()
    {
        // On s'abonne à l'événement de chargement de scène
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // On se désabonne pour éviter les fuites de mémoire
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Exploration_main" || scene.name == "SampleScene")
        {
            // Reset la référence car l'objet a été détruit au changement de scène
            eventButton = null;

            // Toujours reset l'overlay, event actif ou non
            ApplyBlackOverlayOpacity();
            Debug.Log($"[OnSceneLoaded] Scène '{scene.name}' chargée | isEventActive={isEventActive} | activeEventID={activeEventID}");

            if (isEventActive)
            {
                // Retrouve le bouton dans la nouvelle scène
                GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    if (obj.CompareTag("eventbutton"))
                    {
                        eventButton = obj;
                        break;
                    }
                }

                if (eventButton != null)
                {
                    eventButton.SetActive(true);
                    Debug.Log("[OnSceneLoaded] Event bouton réactivé.");
                }
                else
                {
                    Debug.LogWarning("[OnSceneLoaded] Event bouton introuvable !");
                }
            }
        }
    }

    private void ApplyBlackOverlayOpacity()
    {
        GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");

        if (blackOverlay != null)
        {
            SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                Color col = sr.color;

                if (isEventActive && activeEventID == 1)
                {
                    col.a = eventSettings.overlayMaxOpacity;
                    Debug.Log($"[BlackOverlay] Event 1 actif -> opacité appliquée : {eventSettings.overlayMaxOpacity}");
                }
                else
                {
                    col.a = 0f;
                    Debug.Log($"[BlackOverlay] Reset à 0 | isEventActive={isEventActive} | activeEventID={activeEventID}");
                }

                sr.color = col;
            }
            else
            {
                Debug.LogWarning("[BlackOverlay] SpriteRenderer introuvable sur l'objet BlackOverlay !");
            }
        }
        else
        {
            Debug.LogWarning("[BlackOverlay] Aucun objet avec le tag 'BlackOverlay' trouvé dans la scène !");
        }
    }
    IEnumerator RunEvent(int eventID, int durationInMonths)
    {
        isEventActive = true;
        activeEventID = eventID;

        // --- NOUVEAU : Reset du Scroll/Zoom ---
        if (CamMov.Instance != null)
        {
            CamMov.Instance.ResetZoom();
        }

        // --- DÉTERMINATION DE LA DURÉE RÉELLE ---
        int actualDuration = durationInMonths;

        // On cherche dans les invasions
        var invSetting = eventSettings.invasionTypes.Find(e => e.eventID == eventID);
        if (invSetting != null)
        {
            actualDuration = invSetting.durationInMonths;
        }
        else
        {
            // On cherche dans les événements normaux
            var normSetting = eventSettings.normalEvents.Find(e => e.eventID == eventID);
            if (normSetting != null)
            {
                actualDuration = normSetting.durationInMonths;
            }
        }

        // Cas spécial pour la Fête du Corail (ID 3)
        if (eventID == 3)
        {
            actualDuration = eventSettings.coralFestivalDuration;
        }
        // ----------------------------------------

        // Gestion du bouton d'événement
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
        }

        if (eventButton != null) eventButton.SetActive(true);

        // Paramètres visuels et dialogues
        AnimationClip currentAnim = eventSettings.defaultMapAnimation;
        Speech dialogueToUse = null;

        if (invSetting != null)
        {
            dialogueToUse = invSetting.dialogue;
            currentAnim = invSetting.mapAnimation;
        }
        else
        {
            var norm = eventSettings.normalEvents.Find(e => e.eventID == eventID);
            if (norm != null)
            {
                dialogueToUse = norm.dialogue;
                currentAnim = norm.mapAnimation;
            }
        }

        // Affichage du dialogue
        if (SceneManager.GetActiveScene().name == "SampleScene" && dialogueToUse != null && ShowDialogue.Instance != null)
        {
            ShowDialogue.Instance.DialogueBox(dialogueToUse, true);
        }

        // Vérification des instances nécessaires
        if (eventSettings == null || Materials.instance == null || J_TimeManager.Instance == null)
        {
            isEventActive = false; activeEventID = -1; yield break;
        }

        // --- EFFETS DE DÉBUT D'ÉVÉNEMENT ---
        switch (eventID)
        {
            case 0:
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.ActivateTrashWaveEffect();
                if (E_TrashSpawner.Instance != null) E_TrashSpawner.Instance.ActivateTrashWaveEffect();
                break;

            case 1:
                ApplyBlackOverlayOpacity(); // Appelle la nouvelle fonction
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.IncreaseFishSpawnRate();
                break;

            case 2:
            case 4:
            case 5:
            case 12:
                var invData = eventSettings.invasionTypes.Find(e => e.eventID == eventID);
                if (invData != null && invData.prefabs.Length > 0 && E_FishSpawner.Instance != null)
                    E_FishSpawner.Instance.EnableInvasionMode(invData.prefabs[0]);
                break;

            case 3: // Fête du Corail
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.IncreaseFishSpawnRate();
                Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 1f);
                Materials.instance.price = Mathf.Max(Materials.instance.price - 50, 0);
                break;

            case 6:
                foreach (var b in FindObjectsOfType<Builder>()) b.enabled = false;
                Materials.instance.price = Mathf.Max(Materials.instance.price - 50, 0);
                break;

            case 7:
                foreach (var b in FindObjectsOfType<Builder>()) b.cycleDuration *= 1.5f;
                break;

            case 8:
                Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.2f, 0f);
                Building[] buildings = Resources.LoadAll<Building>("Buildings");
                foreach (Building b in buildings)
                {
                    if (b.name == "Station d'épuration" || b.name == "Restaurant") b.time = 20;
                }
                break;

            case 9:
            case 10:
                if (E_FishSpawner.Instance != null)
                {
                    E_FishSpawner.Instance.minSpawnInterval *= 1.2f;
                    E_FishSpawner.Instance.maxSpawnInterval *= 1.2f;
                }
                break;

            case 11:
                Materials.instance.bar_2 = Mathf.Min(Materials.instance.bar_2 + 0.2f, 1f);
                Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.1f, 0f);
                break;

            case 13:
                Materials.instance.price = Mathf.Max(Materials.instance.price - 100, 0);
                Materials.instance.bar_0 = Mathf.Max(Materials.instance.bar_0 - 0.1f, 0f);
                break;

            case 14:
                Materials.instance.bar_2 = Mathf.Max(Materials.instance.bar_2 - 0.2f, 0f);
                Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 0.99f);
                break;

            case 15:
                foreach (var b in FindObjectsOfType<Builder>()) b.cycleDuration *= 0.8f;
                break;

            case 16: Materials.instance.price += 200; break;
            case 17: case 18: Materials.instance.price += 150; break;

            case 19:
                Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.15f, 0.99f);
                Materials.instance.price += 50;
                break;

            case 20:
                Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 0.99f);
                Materials.instance.price = Mathf.Max(Materials.instance.price - 50, 0);
                break;

            case 21:
                Builder[] allBuilders = FindObjectsOfType<Builder>();
                if (allBuilders.Length > 0) allBuilders[Random.Range(0, allBuilders.Length)].enabled = false;
                break;

            case 23:
                Materials.instance.bar_2 = Mathf.Min(Materials.instance.bar_2 + 0.3f, 1f);
                break;
        }

        // --- ATTENTE DE LA DURÉE DE L'ÉVÉNEMENT ---
        int startYear = J_TimeManager.Instance.currentYear;
        int startMonth = J_TimeManager.Instance.currentMonth;
        int monthsPassed = 0;

        while (monthsPassed < actualDuration)
        {
            yield return null;
            monthsPassed = (J_TimeManager.Instance.currentYear - startYear) * 12 + (J_TimeManager.Instance.currentMonth - startMonth);
        }

        // --- EFFETS DE FIN D'ÉVÉNEMENT ---
        switch (eventID)
        {


            case 1:
                isEventActive = false; // Important de le mettre avant pour la fonction
                ApplyBlackOverlayOpacity(); // Remettra l'alpha à 0
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                break;
            case 0:
            case 3:
            case 9:
            case 10:
            case 22:
            case 23:
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.RestoreDefaultSpawnRate();
                if (eventID == 0 && E_TrashSpawner.Instance != null) E_TrashSpawner.Instance.RestoreDefaultTrashSpawnRate();
                if (eventID == 1)
                {
                    GameObject black = GameObject.FindWithTag("BlackOverlay");
                    if (black != null)
                    {
                        SpriteRenderer sr = black.GetComponent<SpriteRenderer>();
                        if (sr != null) { Color c = sr.color; c.a = 0f; sr.color = c; }
                    }
                }
                break;
            case 2:
            case 4:
            case 5:
            case 12:
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.DisableInvasionMode();
                break;
            case 6:
            case 21:
                foreach (var b in FindObjectsOfType<Builder>()) b.enabled = true;
                break;
            case 7:
                foreach (var b in FindObjectsOfType<Builder>()) b.cycleDuration /= 1.5f;
                break;
            case 8:
                foreach (Building b in Resources.LoadAll<Building>("Buildings"))
                    if (b.name == "Station d'épuration" || b.name == "Restaurant") b.time = 10;
                break;
        }

        activeEventID = -1;
        if (eventButton != null) eventButton.SetActive(false);
        isEventActive = false;

        E_CycleEventManager cycleManager = FindObjectOfType<E_CycleEventManager>();
        if (cycleManager != null) cycleManager.EndEvent();
    }
}