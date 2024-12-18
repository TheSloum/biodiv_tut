using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_GameManager : MonoBehaviour
{
    public static E_GameManager instance = null;

    public GameObject oxygenManagerObject;
    private E_OxygenManager oxygenManager;

    void Awake()
    {
        if(E_GameManager.instance == null)
        {
            E_GameManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(E_GameManager.instance != this)
        {
            Destroy(gameObject);
        }

        oxygenManager = oxygenManagerObject.GetComponent<E_OxygenManager>();
    }

    public E_OxygenManager GetOxygenManager()
    {
        return oxygenManager;
    }

    // Méthode pour sauvegarder le jeu
    public void SaveGame()
    {
        E_PlayerController player = FindObjectOfType<E_PlayerController>();
        if(player == null)
        {
            Debug.LogError("E_PlayerController non trouvé dans la scène !");
            return;
        }

        E_SaveData data = new E_SaveData
        {
            playerPosition = player.transform.position,
            currentOxygen = oxygenManager.currentOxygen,
            trashCollected = oxygenManager.trashCollected
        };

        // Sauvegarder le temps actuel
        if (J_TimeManager.Instance != null)
        {
            data.currentDay = J_TimeManager.Instance.currentDay;
            data.currentMonth = J_TimeManager.Instance.currentMonth;
        }

        E_SaveLoadManager.instance.SaveGame(data);
    }

    public void LoadGame()
    {
        E_SaveData data = E_SaveLoadManager.instance.LoadGame();
        if(data != null)
        {
            E_PlayerController player = FindObjectOfType<E_PlayerController>();
            if(player != null)
            {
                player.transform.position = data.playerPosition;
            }

            if(oxygenManager != null)
            {
                oxygenManager.currentOxygen = data.currentOxygen;
                oxygenManager.oxygenSlider.value = data.currentOxygen;
            }

            if(oxygenManager != null)
            {
                oxygenManager.trashCollected = data.trashCollected;
                oxygenManager.UpdateTrashCounterUI();
            }

            // Restaurer le temps
            if (J_TimeManager.Instance != null)
            {
                J_TimeManager.Instance.SetTime(data.currentDay, data.currentMonth);
            }

            Debug.Log("Jeu chargé avec succès !");
        }
        else
        {
            Debug.LogWarning("Aucune donnée de sauvegarde à charger.");
        }
    }


    void Start()
    {
        // Initialisation supplémentaire si nécessaire
    }

    void Update()
    {
        // Détecter l'appui sur "S" pour sauvegarder
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        // Détecter l'appui sur "L" pour charger
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }
}
