using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class J_DisplayDateTime : MonoBehaviour
{
    private TextMeshProUGUI dateText;

    void Awake() => FindDateText();

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void Update()
    {
        if (dateText == null) return;

        if (J_TimeManager.Instance != null)
        {
            int day = J_TimeManager.Instance.currentDay;
            int month = J_TimeManager.Instance.GetCurrentMonth();
            int year = J_TimeManager.Instance.GetCurrentYear();
            
            // Formatage "Jour 15, Mois 3, Année 2"
            dateText.text = $"J {day}, M {month}, A {year}";
        }
        else
        {
            dateText.text = "Time Manager non trouvé.";
        }
    }

    private void FindDateText()
    {
        GameObject dateTextObject = GameObject.FindGameObjectWithTag("DateText");
        
        if (dateTextObject != null)
        {
            dateText = dateTextObject.GetComponent<TextMeshProUGUI>();
            if (dateText == null)
            {
                Debug.LogWarning("Le GameObject avec le tag 'DateText' n'a pas de composant TextMeshProUGUI.");
            }
        }
        else
        {
            Debug.LogWarning("Aucun GameObject avec le tag 'DateText' trouvé dans la scène.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindDateText();
    }
}