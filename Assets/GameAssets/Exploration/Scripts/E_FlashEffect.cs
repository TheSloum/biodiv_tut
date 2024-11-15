using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class E_FlashEffect : MonoBehaviour
{
    public static E_FlashEffect Instance;

    [Header("Flash Settings")]
    public Image flashImage; // Assign FlashOverlay ici via l'Inspector
    public float flashDuration = 0.2f; // Durée totale du flash
    public float flashAlpha = 1f; // Opacité maximale du flash

    private void Awake()
    {
        // Implémenter le Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optionnel : persiste entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Méthode publique pour déclencher le flash
    public void TriggerFlash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        float halfDuration = flashDuration / 2f;

        // Fade In
        float elapsed = 0f;
        Color color = flashImage.color;
        while (elapsed < halfDuration)
        {
            color.a = Mathf.Lerp(0f, flashAlpha, elapsed / halfDuration);
            flashImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = flashAlpha;
        flashImage.color = color;

        // Fade Out
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            color.a = Mathf.Lerp(flashAlpha, 0f, elapsed / halfDuration);
            flashImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = 0f;
        flashImage.color = color;
    }
}
