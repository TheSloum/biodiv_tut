using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class E_EventDefinitions
{
    public string message;
    public Color messageColor;
    public float textFadeInDuration;
    public float textVisibleDuration;
    public float textFadeOutDuration;
    public float eventDuration;
    public UnityEvent OnEventStart;
    public UnityEvent OnEventEnd;
}

public class E_Event : MonoBehaviour
{
    [Header("Event Definitions")]
    // Nous remplissons le tableau en dur dans le code
    public E_EventDefinition[] eventDefinitions;

    [Header("UI & Overlay")]
    public SpriteRenderer blackOverlayRenderer;
    public TextMeshProUGUI eventText;
    public float overlayFadeInDuration = 3f;
    public float overlayFadeOutDuration = 2f;

    private bool isEventActive = false;

    void Start()
    {
        // Initialisation de l'UI
        if (eventText != null)
        {
            eventText.color = new Color(eventText.color.r, eventText.color.g, eventText.color.b, 0f);
            eventText.gameObject.SetActive(false);
        }
        if (blackOverlayRenderer != null)
        {
            blackOverlayRenderer.gameObject.SetActive(false);
        }
    }

    // Méthode pour déclencher un événement via son index dans le tableau eventDefinitions
    public void TriggerEvent(int eventIndex)
    {
        if (isEventActive || eventDefinitions == null || eventIndex < 0 || eventIndex >= eventDefinitions.Length)
            return;
        StartCoroutine(RunEvent(eventDefinitions[eventIndex]));
    }

    IEnumerator RunEvent(E_EventDefinition eventDef)
    {
        isEventActive = true;

        // Optionnel : fade in de l'overlay
        if (blackOverlayRenderer != null)
            yield return StartCoroutine(FadeInOverlay());

        // Affichage du texte avec effet fade in/out
        yield return StartCoroutine(DisplayEventText(eventDef.message, eventDef.messageColor, eventDef.textFadeInDuration, eventDef.textVisibleDuration, eventDef.textFadeOutDuration));

        // Déclenchement du callback de début d'événement
        eventDef.OnEventStart?.Invoke();

        // Durée de l'événement
        yield return new WaitForSeconds(eventDef.eventDuration);

        // Déclenchement du callback de fin d'événement
        eventDef.OnEventEnd?.Invoke();

        // Optionnel : fade out de l'overlay
        if (blackOverlayRenderer != null)
            yield return StartCoroutine(FadeOutOverlay());

        isEventActive = false;
    }

    IEnumerator DisplayEventText(string message, Color color, float fadeInDuration, float visibleDuration, float fadeOutDuration)
    {
        eventText.text = message;
        eventText.color = new Color(color.r, color.g, color.b, 0f);
        eventText.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            eventText.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        eventText.color = new Color(color.r, color.g, color.b, 1f);
        yield return new WaitForSeconds(visibleDuration);

        timer = 0f;
        while (timer < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            eventText.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        eventText.gameObject.SetActive(false);
    }

    IEnumerator FadeInOverlay()
    {
        blackOverlayRenderer.gameObject.SetActive(true);
        Color c = blackOverlayRenderer.color;
        c.a = 0f;
        blackOverlayRenderer.color = c;
        float timer = 0f;
        while (timer < overlayFadeInDuration)
        {
            c.a = Mathf.Lerp(0f, 1f, timer / overlayFadeInDuration);
            blackOverlayRenderer.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator FadeOutOverlay()
    {
        Color c = blackOverlayRenderer.color;
        float startAlpha = c.a;
        float timer = 0f;
        while (timer < overlayFadeOutDuration)
        {
            c.a = Mathf.Lerp(startAlpha, 0f, timer / overlayFadeOutDuration);
            blackOverlayRenderer.color = c;
            timer += Time.deltaTime;
            yield return null;
        }
        blackOverlayRenderer.gameObject.SetActive(false);
    }
}
