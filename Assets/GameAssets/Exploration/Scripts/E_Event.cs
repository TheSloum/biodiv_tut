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
    public int coralFestivalCycle = 4;

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

        Debug.Log($"Test Festival -> Année: {currentYear} (Condition OK: {isRightYear})");

        return isRightYear;
    }

    IEnumerator RunEvent(int eventID, int durationInMonths)
    {
        isEventActive = true;
        activeEventID = eventID;

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

        AnimationClip currentAnim = eventSettings.defaultMapAnimation;
        Speech dialogueToUse = null;

        var inv = eventSettings.invasionTypes.Find(e => e.eventID == eventID);
        if (inv != null)
        {
            dialogueToUse = inv.dialogue;
            currentAnim = inv.mapAnimation;
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

        if (SceneManager.GetActiveScene().name == "SampleScene" && dialogueToUse != null && ShowDialogue.Instance != null)
        {
            ShowDialogue.Instance.DialogueBox(dialogueToUse, true);
        }

        if (eventSettings == null || Materials.instance == null || J_TimeManager.Instance == null)
        {
            isEventActive = false; activeEventID = -1; yield break;
        }

        switch (eventID)
        {
            case 0:
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.ActivateTrashWaveEffect();
                if (E_TrashSpawner.Instance != null) E_TrashSpawner.Instance.ActivateTrashWaveEffect();
                break;

            case 1:
                GameObject blackOverlay = GameObject.FindWithTag("BlackOverlay");
                if (blackOverlay != null)
                {
                    SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color col = sr.color;
                        col.a = eventSettings.overlayMaxOpacity;
                        sr.color = col;
                    }
                }
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

            case 3:
                if (E_FishSpawner.Instance != null) E_FishSpawner.Instance.IncreaseFishSpawnRate();
                Materials.instance.bar_0 = Mathf.Min(Materials.instance.bar_0 + 0.1f, 0.99f);
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

        int startYear = J_TimeManager.Instance.currentYear;
        int startMonth = J_TimeManager.Instance.currentMonth;
        int monthsPassed = 0;
        while (monthsPassed < durationInMonths)
        {
            yield return null;
            monthsPassed = (J_TimeManager.Instance.currentYear - startYear) * 12 + (J_TimeManager.Instance.currentMonth - startMonth);
        }

        switch (eventID)
        {
            case 0:
            case 1:
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