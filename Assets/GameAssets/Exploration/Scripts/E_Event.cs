using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class E_Event : MonoBehaviour
{
    [Header("Event Definitions")]
    public E_EventDefinition[] eventDefinitions;

    [Header("UI & Overlay")]
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

        // Affichage du texte avec effet fade in/out
        yield return StartCoroutine(DisplayEventText(eventDef.message, eventDef.messageColor, eventDef.textFadeInDuration, eventDef.textVisibleDuration, eventDef.textFadeOutDuration));

        // Déclenchement du callback de début d'événement
        eventDef.OnEventStart?.Invoke();

        // Durée de l'événement
        yield return new WaitForSeconds(eventDef.eventDuration);

        // Déclenchement du callback de fin d'événement
        eventDef.OnEventEnd?.Invoke();



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


}
