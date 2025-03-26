using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ControlManager : MonoBehaviour
{
    [Header("⚙ Paramètres du mode Dyslexie")]
    public Toggle dyslexiaToggle; // Glisser le Toggle ici
    public TMP_FontAsset normalFont; // Police normale
    public TMP_FontAsset dyslexiaFont; // Police dyslexie
    public TMP_Text[] textsToChange; // Liste des textes à modifier

    private const string DyslexiaPrefKey = "DyslexiaMode"; // Clé de sauvegarde PlayerPrefs

    void Start()
    {
        // 📌 Charger l'état du mode dyslexie sauvegardé
        if (PlayerPrefs.HasKey(DyslexiaPrefKey))
        {
            bool isDyslexiaEnabled = PlayerPrefs.GetInt(DyslexiaPrefKey) == 1;
            dyslexiaToggle.isOn = isDyslexiaEnabled; // Appliquer l'état du Toggle
            ApplyDyslexiaMode(isDyslexiaEnabled); // Appliquer immédiatement le mode dyslexie
            Materials.instance.dys = isDyslexiaEnabled;
        }

        // 📌 Ajouter un écouteur pour sauvegarder et appliquer les changements
        dyslexiaToggle.onValueChanged.AddListener(OnDyslexiaToggleChanged);
    }

    void OnDyslexiaToggleChanged(bool isEnabled)
    {
        // 📌 Sauvegarder l'état dans PlayerPrefs
        PlayerPrefs.SetInt(DyslexiaPrefKey, isEnabled ? 1 : 0);
        PlayerPrefs.Save(); // S'assurer que les données sont bien enregistrées

        // 📌 Appliquer les changements de police
        ApplyDyslexiaMode(isEnabled);
    }

    void ApplyDyslexiaMode(bool isEnabled)
    {
            Materials.instance.dys = isEnabled;
        foreach (TMP_Text txt in textsToChange)
        {
            if (txt != null)
            {
                txt.font = isEnabled ? dyslexiaFont : normalFont;
            }
        }
    }
}
