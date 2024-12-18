using UnityEngine;
using UnityEngine.UI;

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


    private void Start()
    {

        buttonSon.onClick.AddListener(ShowSonTab);
        buttonAffichage.onClick.AddListener(ShowAffichageTab);
        buttonControl.onClick.AddListener(ShowControlTab);


        ShowSonTab();
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

        Debug.Log("Onglet Control activé");
    }

}

