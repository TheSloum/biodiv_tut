using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Materials instance;

    // mat_0: Bois, mat_1: Pierre, mat_2: Fer
    public int mat_0 = 500; // Bois
    public int mat_1 = 500; // Pierre
    public int mat_2 = 500; // Fer
    public int price = 500;
    public float bar_0 = 0.5f;
    public float bar_1 = 0.5f;
    public float bar_2 = 0.5f;

    // Références UI (optionnel)
    public UnityEngine.UI.Text woodText;
    public UnityEngine.UI.Text stoneText;
    public UnityEngine.UI.Text ironText;

    private void Awake()
    {
        if(instance == null)
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
            Debug.Log($"Materials chargés: mat_0 (Bois) = {mat_0}, mat_1 (Pierre) = {mat_1}, mat_2 (Fer) = {mat_2}");

            // Mettre à jour l'UI si nécessaire
            UpdateUI();
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
        Debug.Log($"Bois collecté : +{amount} (Total : {mat_0})");
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
        Debug.Log($"Pierre collectée : +{amount} (Total : {mat_1})");
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
        Debug.Log($"Fer collecté : +{amount} (Total : {mat_2})");
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

    // Méthode pour mettre à jour l'UI
    void UpdateUI()
    {
        if (woodText != null)
            woodText.text = $"Bois : {mat_0}";
        if (stoneText != null)
            stoneText.text = $"Pierre : {mat_1}";
        if (ironText != null)
            ironText.text = $"Fer : {mat_2}";
    }
}