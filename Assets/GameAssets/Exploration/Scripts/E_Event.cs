using UnityEngine;
using TMPro; // Import nécessaire pour TextMeshPro
using System.Collections;

public class E_Event : MonoBehaviour
{
    [Header("Overlay Noir")]
    [Tooltip("SpriteRenderer du GameObject BlackOverlay couvrant l'écran.")]
    public SpriteRenderer blackOverlayRenderer;

    [Header("UI Elements")]
    [Tooltip("Texte affichant le nom de l'événement.")]
    public TextMeshProUGUI eventText; // Changement de Text à TextMeshProUGUI

    [Header("Phase Pétrole Configuration")]
    [Tooltip("Durée de l'événement en secondes.")]
    public float eventDuration = 10f;

    [Tooltip("Opacité minimale de l'overlay (entre 0 et 1).")]
    [Range(0f, 1f)]
    public float minOpacity = 0.8f;

    [Tooltip("Opacité maximale de l'overlay (entre 0 et 1).")]
    [Range(0f, 1f)]
    public float maxOpacity = 1f;

    [Tooltip("Facteur de ralentissement du joueur (1 = vitesse normale).")]
    [Range(0f, 1f)]
    public float slowFactor = 0.8f;

    [Header("Event Text Configuration")]
    [Tooltip("Durée du fondu d'apparition du texte en secondes.")]
    public float textFadeInDuration = 1f;

    [Tooltip("Durée pendant laquelle le texte reste visible en secondes.")]
    public float textVisibleDuration = 2f;

    [Tooltip("Durée du fondu de disparition du texte en secondes.")]
    public float textFadeOutDuration = 1f;

    [Header("Overlay Fade Configuration")]
    [Tooltip("Durée du fondu d'apparition de l'overlay en secondes.")]
    public float overlayFadeInDuration = 3f;

    private bool isEventActive = false;
    private E_PlayerController playerController; // Référence au script de contrôle du joueur

    void Start()
    {
        // Vérifier que les éléments sont assignés
        if (blackOverlayRenderer == null)
        {
            Debug.LogError("Black Overlay Renderer n'est pas assigné dans le script E_Event.");
        }

        if (eventText == null)
        {
            Debug.LogError("Event Text n'est pas assigné dans le script E_Event.");
        }

        // Assumer que le joueur a le tag "Player" et récupérer son script de contrôle
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<E_PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("Le joueur n'a pas de script E_PlayerController attaché.");
            }
        }
        else
        {
            Debug.LogError("Aucun GameObject avec le tag 'Player' trouvé dans la scène.");
        }

        // Initialiser l'overlay à transparent
        if (blackOverlayRenderer != null)
        {
            Color initialColor = blackOverlayRenderer.color;
            initialColor.a = 0f;
            blackOverlayRenderer.color = initialColor;
            blackOverlayRenderer.gameObject.SetActive(false);
        }

        // Initialiser le texte à transparent et inactif
        if (eventText != null)
        {
            Color initialTextColor = eventText.color;
            initialTextColor.a = 0f;
            eventText.color = initialTextColor;
            eventText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Détecter l'appui sur la touche "E" pour déclencher ou arrêter l'événement
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

        // Afficher le texte de l'événement avec fondu
        StartCoroutine(FadeInText());
        yield return new WaitForSeconds(textFadeInDuration + textVisibleDuration + textFadeOutDuration);

        // Appliquer le ralentissement au joueur
        if (playerController != null)
        {
            playerController.moveForce *= slowFactor; // Utilise moveForce au lieu de moveSpeed
        }

        // Afficher l'overlay noir avec fondu
        StartCoroutine(FadeInOverlay());

        // Durée de l'événement
        float elapsedTime = 0f;
        while (elapsedTime < eventDuration)
        {
            // Changer l'opacité de l'overlay de manière aléatoire
            float newOpacity = Random.Range(minOpacity, maxOpacity);
            StartCoroutine(FadeOverlayTo(newOpacity, 1f)); // Changement de l'opacité sur 1 seconde
            yield return new WaitForSeconds(1f); // Attendre avant de changer à nouveau
            elapsedTime += 1f;
        }

        // Fin de l'événement
        EndPhasePetrole();
    }

    void EndPhasePetrole()
    {
        isEventActive = false;

        // Retirer le ralentissement du joueur
        if (playerController != null)
        {
            playerController.moveForce /= slowFactor; // Utilise moveForce au lieu de moveSpeed
        }

        // Cacher l'overlay noir avec fondu
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

        // Attendre la durée de visibilité
        yield return new WaitForSeconds(textVisibleDuration);

        // Fondu de disparition
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
        float fadeOutDuration = 3f; // Durée du fondu de disparition

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
}
