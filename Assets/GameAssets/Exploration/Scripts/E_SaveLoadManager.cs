using System.IO;
using UnityEngine;

public class E_SaveLoadManager : MonoBehaviour
{
    public static E_SaveLoadManager instance = null;
    private string saveFilePath;

    void Awake()
    {
        // Implémenter le Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Définir le chemin du fichier de sauvegarde
        #if UNITY_WEBGL && !UNITY_EDITOR
    string saveFolderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
    string saveFolderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif
    }

    // Méthode pour sauvegarder le jeu
    public void SaveGame(E_SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Jeu sauvegardé à : " + saveFilePath);
    }

    // Méthode pour charger le jeu
    public E_SaveData LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            E_SaveData data = JsonUtility.FromJson<E_SaveData>(json);
            Debug.Log("Jeu chargé depuis : " + saveFilePath);
            return data;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée à : " + saveFilePath);
            return null;
        }
    }
}
