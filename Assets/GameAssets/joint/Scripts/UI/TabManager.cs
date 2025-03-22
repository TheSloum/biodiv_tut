using UnityEngine;
using UnityEngine.UI;
using TMPro; // Ajout pour gérer TextMeshPro

public class TabManager : MonoBehaviour
{
    [Header("GameObjects des Onglets")]
    public GameObject sonGameObject;
    public GameObject affichageGameObject;
    public GameObject controlGameObject;

    [Header("Boutons des Onglets")]
    public Button buttonSon;
    public Button buttonAffichage;
    public Button buttonControl;

    [Header("Textes des Boutons")]
    public TextMeshProUGUI textSon;
    public TextMeshProUGUI textAffichage;

    [Header("Sprites des Boutons")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private void Start()
    {
        // Ajouter les événements aux boutons
        buttonSon.onClick.AddListener(ShowSonTab);
        buttonAffichage.onClick.AddListener(ShowAffichageTab);
        buttonControl.onClick.AddListener(ShowControlTab);

        // Afficher l'onglet par défaut
        ShowSonTab();
    }

    private void Update()
    {
        // Met à jour les sprites et les couleurs de texte
        UpdateButtonAppearance();
    }

    private void ShowSonTab()
    {
        sonGameObject.SetActive(true);
        affichageGameObject.SetActive(false);
        controlGameObject.SetActive(false);
        Debug.Log("Onglet Son activé");
    }

    private void ShowAffichageTab()
    {
        sonGameObject.SetActive(false);
        affichageGameObject.SetActive(true);
        controlGameObject.SetActive(false);
        Debug.Log("Onglet Affichage activé");
    }

    private void ShowControlTab()
    {
        sonGameObject.SetActive(false);
        affichageGameObject.SetActive(false);
        controlGameObject.SetActive(true);
        Debug.Log("Onglet Contrôle activé");
    }

    private void UpdateButtonAppearance()
    {
        // Vérifier quel onglet est actif et mettre à jour les sprites et la couleur du texte
        UpdateButton(buttonSon, textSon, sonGameObject.activeSelf);
        UpdateButton(buttonAffichage, textAffichage, affichageGameObject.activeSelf);
    }

    private void UpdateButton(Button button, TextMeshProUGUI text, bool isActive)
    {
        button.GetComponent<Image>().sprite = isActive ? activeSprite : inactiveSprite;
        text.color = isActive ? Color.white : Color.black;
    }
}
