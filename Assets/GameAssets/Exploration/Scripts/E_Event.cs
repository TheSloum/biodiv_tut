using UnityEngine;
using TMPro;
using System.Collections;

public class E_Event : MonoBehaviour
{
    public SpriteRenderer blackOverlayRenderer;
    public TextMeshProUGUI eventText;

    public float eventDuration = 10f;
    public float minOpacity = 0.8f;
    public float maxOpacity = 1f;
    public float slowFactor = 0.8f;

    public float textFadeInDuration = 1f;
    public float textVisibleDuration = 2f;
    public float textFadeOutDuration = 1f;

    public float overlayFadeInDuration = 3f;

    private bool isEventActive = false;
    private E_PlayerController playerController;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<E_PlayerController>();
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'Player' trouvé dans la scène.");
        }

        // CHANGEMENT : Réinitialiser l'état visuel de l'événement
        if (blackOverlayRenderer != null)
        {
            Color initialColor = blackOverlayRenderer.color;
            initialColor.a = 0f;
            blackOverlayRenderer.color = initialColor;
            blackOverlayRenderer.gameObject.SetActive(false);
        }

        if (eventText != null)
        {
            Color initialTextColor = eventText.color;
            initialTextColor.a = 0f;
            eventText.color = initialTextColor;
            eventText.gameObject.SetActive(false);
        }

        isEventActive = false;
    }

    void Update()
    {
        // Contrôle de l'événement (touche E), code existant...
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isEventActive)
            {
                StartCoroutine(StartPhasePetrole());
            }
            else
            {
                StopAllCoroutines();
                EndPhasePetrole();
            }
        }
    }

    IEnumerator StartPhasePetrole()
    {
        isEventActive = true;
        StartCoroutine(FadeInText());
        yield return new WaitForSeconds(textFadeInDuration + textVisibleDuration + textFadeOutDuration);

        if (playerController != null)
        {
            playerController.moveForce *= slowFactor;
        }

        StartCoroutine(FadeInOverlay());

        float elapsedTime = 0f;
        while (elapsedTime < eventDuration)
        {
            float newOpacity = Random.Range(minOpacity, maxOpacity);
            StartCoroutine(FadeOverlayTo(newOpacity, 1f));
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        EndPhasePetrole();
    }

    void EndPhasePetrole()
    {
        isEventActive = false;

        if (playerController != null)
        {
            playerController.moveForce /= slowFactor;
        }

        StartCoroutine(FadeOutOverlay());
    }

    IEnumerator FadeInText()
    {
        eventText.gameObject.SetActive(true);
        Color originalColor = eventText.color;
        originalColor.a = 0f;
        eventText.color = originalColor;

        float elapsedTime = 0f;
        while (elapsedTime < textFadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / textFadeInDuration);
            eventText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        eventText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        yield return new WaitForSeconds(textVisibleDuration);

        elapsedTime = 0f;
        while (elapsedTime < textFadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / textFadeOutDuration);
            eventText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        eventText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        eventText.gameObject.SetActive(false);
    }

    IEnumerator FadeInOverlay()
    {
        blackOverlayRenderer.gameObject.SetActive(true);
        Color originalColor = blackOverlayRenderer.color;
        originalColor.a = 0f;
        blackOverlayRenderer.color = originalColor;

        float elapsedTime = 0f;
        while (elapsedTime < overlayFadeInDuration)
        {
            float alpha = Mathf.Lerp(0f, minOpacity, elapsedTime / overlayFadeInDuration);
            blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, minOpacity);
    }

    IEnumerator FadeOverlayTo(float targetOpacity, float duration)
    {
        Color originalColor = blackOverlayRenderer.color;
        float startOpacity = originalColor.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startOpacity, targetOpacity, elapsedTime / duration);
            blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, targetOpacity);
    }

    IEnumerator FadeOutOverlay()
    {
        Color originalColor = blackOverlayRenderer.color;
        float startOpacity = originalColor.a;
        float fadeOutDuration = 3f; 

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(startOpacity, 0f, elapsedTime / fadeOutDuration);
            blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        blackOverlayRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        blackOverlayRenderer.gameObject.SetActive(false);
    }

    // CHANGEMENT : Méthode pour réinitialiser l'état de l'événement
    public void ResetEvent()
    {
        StopAllCoroutines();
        isEventActive = false;

        if (blackOverlayRenderer != null)
        {
            Color c = blackOverlayRenderer.color;
            c.a = 0f;
            blackOverlayRenderer.color = c;
            blackOverlayRenderer.gameObject.SetActive(false);
        }

        if (eventText != null)
        {
            Color tc = eventText.color;
            tc.a = 0f;
            eventText.color = tc;
            eventText.gameObject.SetActive(false);
        }
    }
}
