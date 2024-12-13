using System.IO;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameDataManager : MonoBehaviour
{
   [Header("References")]
    public GameObject saveButtonPrefab;
    public Transform saveListParent; 
    public GameDataSaver gameDataSaver; 


    private void Awake()
    {
        LoadSaveFiles();
    }

void OnEnable()
    {
        
        LoadSaveFiles();
    }

    public void LoadSaveFiles()
    {
        // Clear existing save buttons
        foreach (Transform child in saveListParent)
        {
            if(!child.gameObject.CompareTag("ButtonPriority")){
            Destroy(child.gameObject);
            }
        }

        // Get all save files
        string saveFolderPath = Path.Combine(Application.dataPath, "Sauvegardes");
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.LogWarning("Save folder does not exist. Creating a new one.");
            Directory.CreateDirectory(saveFolderPath);
        }

        string[] saveFiles = Directory.GetFiles(saveFolderPath, "GameData_*.json");
        int index = 0;

        foreach (string saveFile in saveFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(saveFile);
            string saveDate = fileName.Replace("GameData_", "");

            // Instantiate a new button
            GameObject saveButton = Instantiate(saveButtonPrefab, saveListParent);
            saveButton.transform.localPosition -= new Vector3(0, index * 50, 0); // Offset each button vertically
            index++;

            
            TMP_Text textComponent = saveButton.transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
        if (textComponent != null)
        {
            textComponent.text = $"Sauvegarde du {saveDate}";
        } else {Debug.Log("pq");}

            // Configure the buttons
            var buttons = saveButton.GetComponentsInChildren<UnityEngine.UI.Button>();
            if (buttons.Length >= 2)
            {
                buttons[0].onClick.AddListener(() => gameDataSaver.LoadData(saveDate)); // Load button
                buttons[1].onClick.AddListener(() => DeleteSave(saveFile, saveButton)); // Delete button
            }
        }
    }

    private void DeleteSave(string filePath, GameObject saveButton)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else
        {
        }

        // Remove the button from the list
        Destroy(saveButton);
    }
    }

