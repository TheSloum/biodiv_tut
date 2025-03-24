using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class E_GameManager : MonoBehaviour
{
    public static E_GameManager instance = null;

    public GameObject oxygenManagerObject;
    private E_OxygenManager oxygenManager;

    public List<bool> fishUnlockData = new List<bool>(new bool[23]);

    public void ResetState()
    {
        if (gameObject.scene.rootCount == 0) // Vérifie si c'est un prefab (non dans la scène)
        {
            Debug.Log("❌ E_GameManager n'est pas dans la scène, pas de reset.");
            return;
        }

        Debug.Log("🔄 Réinitialisation de E_GameManager...");

        // Réinitialiser les données du gestionnaire d'oxygène
        if (oxygenManager != null)
        {
            oxygenManager.currentOxygen = oxygenManager.maxOxygen; // Remet l'oxygène au max
            oxygenManager.trashCollected = 0;
            oxygenManager.UpdateTrashCounterUI();
        }

        // Réinitialiser les données des poissons
        fishUnlockData = new List<bool>(new bool[23]);

        Debug.Log("✅ E_GameManager réinitialisé !");
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Ajout du listener
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Rechercher l'object OxygenManager dans la nouvelle scène
        oxygenManagerObject = GameObject.FindWithTag("OxygenManager"); // Assurez-vous que votre OxygenManager a un tag
        if (oxygenManagerObject != null)
        {
            oxygenManager = oxygenManagerObject.GetComponent<E_OxygenManager>();
        }
    }

    public E_OxygenManager GetOxygenManager()
    {
        return oxygenManager;
    }


    public void SaveGame()
    {
        E_PlayerController player = FindObjectOfType<E_PlayerController>();
        if (player == null)
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

        // Sauvegarde du temps
        if (J_TimeManager.Instance != null)
        {
            data.currentDay = J_TimeManager.Instance.currentDay;
            data.currentMonth = J_TimeManager.Instance.GetCurrentMonth();
            data.currentYear = J_TimeManager.Instance.GetCurrentYear();
        }

        E_SaveLoadManager.instance.SaveGame(data);
        Debug.Log("Jeu sauvegardé !");
    }

    public void LoadGame()
    {
        E_SaveData data = E_SaveLoadManager.instance.LoadGame();
        if (data != null)
        {
            E_PlayerController player = FindObjectOfType<E_PlayerController>();
            if (player != null)
            {
                player.transform.position = data.playerPosition;
            }

            if (oxygenManager != null)
            {
                oxygenManager.currentOxygen = data.currentOxygen;
                oxygenManager.oxygenSlider.value = data.currentOxygen;
                oxygenManager.trashCollected = data.trashCollected;
                oxygenManager.UpdateTrashCounterUI();
            }

            // Restauration du temps
            if (J_TimeManager.Instance != null)
            {
                J_TimeManager.Instance.SetTime(
                    data.currentDay,
                    data.currentMonth,
                    data.currentYear
                );
            }

            Debug.Log("Jeu chargé avec succès !");
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée.");
        }
    }

    public void UnlockFishData(int index)
    { //Rajout d'une fonction pour s'assurer que les poissons s'unlockent (la sauvegarde de retour à la gestion s'effectue avant l'exploration, donc on revient à l'état avant les unlocks de poisson)
        fishUnlockData[index] = true;
    }

    void Update()
    {
        // Touches de debug
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }
}