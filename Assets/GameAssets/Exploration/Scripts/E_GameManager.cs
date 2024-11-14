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

        E_SaveLoadManager.instance.SaveGame(data);
    }

    // Méthode pour charger le jeu
    public void LoadGame()
    {
        E_SaveData data = E_SaveLoadManager.instance.LoadGame();
        if(data != null)
        {
            // Restaurer la position du joueur
            E_PlayerController player = FindObjectOfType<E_PlayerController>();
            if(player != null)
            {
                player.transform.position = data.playerPosition;
            }

            // Restaurer l'oxygène
            if(oxygenManager != null)
            {
                oxygenManager.currentOxygen = data.currentOxygen;
                oxygenManager.oxygenSlider.value = data.currentOxygen;
            }

            // Restaurer le compteur de Trash
            if(oxygenManager != null)
            {
                oxygenManager.trashCollected = data.trashCollected;
                oxygenManager.UpdateTrashCounterUI();
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
