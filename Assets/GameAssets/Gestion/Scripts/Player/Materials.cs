using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Importer le namespace TextMeshPro

public class Materials : MonoBehaviour
{
    public static Materials instance;

    [HideInInspector]
    public bool isLoad = false;

    public string townName;

    // mat_0: Bois, mat_1: Pierre, mat_2: Fer
    public int mat_0 = 500; // Bois (Total)
    public int mat_1 = 500; // Pierre (Total)
    public int mat_2 = 500; // Fer (Total)
    public int price = 500; // MONEYYYY
    public float bar_0 = 0.5f;
    public float bar_1 = 0.5f;
    public float bar_2 = 0.5f;

    // Références UI (modifiées pour TextMeshProUGUI)
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI stoneText;
    public TextMeshProUGUI ironText;

    // Variables pour suivre les matériaux collectés durant la session
    [HideInInspector]
    public int sessionWood = 0;
    [HideInInspector]
    public int sessionStone = 0;
    [HideInInspector]
    public int sessionIron = 0;



    [HideInInspector]
    public bool canMove = true;

    public bool textDone = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Materials.instance initialisé.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances de Materials détectées et détruites.");
        }
    }

    void Start()
    {
        // Charger les matériaux depuis GameDataSaver
        if (GameDataSaver.instance != null)
        {
            mat_0 = GameDataSaver.instance.mat_0;
            mat_1 = GameDataSaver.instance.mat_1;
            mat_2 = GameDataSaver.instance.mat_2;
            price = GameDataSaver.instance.price;
            Debug.Log($"Materials chargés: mat_0 (Bois) = {mat_0}, mat_1 (Pierre) = {mat_1}, mat_2 (Fer) = {mat_2}, price = {price}");

            // Mettre à jour l'UI avec les totaux si nécessaire
            // Si vous souhaitez également afficher les totaux pendant le jeu, vous pouvez appeler UpdateTotalUI()
            UpdateTotalUI();

            // Réinitialiser les compteurs de session au début
            ResetSessionCounts();
        }
        else
        {
            Debug.LogWarning("GameDataSaver.instance est null dans Materials.Start()");
        }
    }

    void Update()
    {
        // Clamp des valeurs des barres
        bar_0 = Mathf.Clamp(bar_0, 0f, 0.99f);
        bar_1 = Mathf.Clamp(bar_1, 0f, 0.99f);
        bar_2 = Mathf.Clamp(bar_2, 0f, 0.99f);
    }

    // Méthodes pour ajouter des matériaux
    public void AddWood(int amount)
    {
        mat_0 += amount;
        sessionWood += amount; // Incrémenter le compteur de session
        Debug.Log($"Bois collecté : +{amount} (Total : {mat_0}, Session : {sessionWood})");
        UpdateUI();

        // Mettre à jour GameDataSaver et sauvegarder
        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_0 = mat_0;
            GameDataSaver.instance.SaveData();
        }
        else
        {
            Debug.LogWarning("GameDataSaver.instance est null dans AddWood()");
        }
    }

    public void AddStone(int amount)
    {
        mat_1 += amount;
        sessionStone += amount; // Incrémenter le compteur de session
        Debug.Log($"Pierre collectée : +{amount} (Total : {mat_1}, Session : {sessionStone})");
        UpdateUI();

        // Mettre à jour GameDataSaver et sauvegarder
        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_1 = mat_1;
            GameDataSaver.instance.SaveData();
        }
        else
        {
            Debug.LogWarning("GameDataSaver.instance est null dans AddStone()");
        }
    }

    public void AddIron(int amount)
    {
        mat_2 += amount;
        sessionIron += amount; // Incrémenter le compteur de session
        Debug.Log($"Fer collecté : +{amount} (Total : {mat_2}, Session : {sessionIron})");
        UpdateUI();

        // Mettre à jour GameDataSaver et sauvegarder
        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_2 = mat_2;
            GameDataSaver.instance.SaveData();
        }
        else
        {
            Debug.LogWarning("GameDataSaver.instance est null dans AddIron()");
        }
    }

    // Méthode pour réinitialiser les compteurs de session
    public void ResetSessionCounts()
    {
        sessionWood = 0;
        sessionStone = 0;
        sessionIron = 0;
        Debug.Log("Compteurs de session réinitialisés.");
    }

    // Méthode pour mettre à jour l'UI avec les compteurs de session
    void UpdateUI()
    {
        if (woodText != null)
            woodText.text = $": {sessionWood}";
        else
            Debug.LogWarning("woodText n'est pas assigné dans Materials.UpdateUI()");

        if (stoneText != null)
            stoneText.text = $": {sessionStone}";
        else
            Debug.LogWarning("stoneText n'est pas assigné dans Materials.UpdateUI()");

        if (ironText != null)
            ironText.text = $": {sessionIron}";
        else
            Debug.LogWarning("ironText n'est pas assigné dans Materials.UpdateUI()");
    }

    // (Optionnel) Méthode pour mettre à jour l'UI avec les totaux
    public void UpdateTotalUI()
    {
        // Si vous avez des UI pour afficher les totaux, implémentez cette méthode
        // Exemple :
        // totalWoodText.text = $"Total Bois : {mat_0}";
        // totalStoneText.text = $"Total Pierre : {mat_1}";
        // totalIronText.text = $"Total Fer : {mat_2}";
    }
}
