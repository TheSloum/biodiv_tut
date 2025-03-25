using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameDataManager : MonoBehaviour
{
    [Header("References")]
    public GameObject saveButtonPrefab;
    public Transform saveListParent;
    public GameDataSaver gameDataSaver;
    public Scroll scroll;

public GameObject loadingObject; 
    private void Awake()
    {
        
    loadingObject = GameObject.Find("loadingScreen");
        LoadSaveFiles();
    }

    void OnEnable()
    {
        LoadSaveFiles();
        scroll.objectHeight = scroll.GetObjectHeight();
        gameDataSaver = Materials.instance.GetComponent<GameDataSaver>();
    }


private IEnumerator LoadSceneAsync(string sceneName)
    {
        loadingObject.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            float progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            yield return null; 
        }

    }
    public void LoadSaveFiles()
    {
        // Clear existing save buttons
        foreach (Transform child in saveListParent)
        {
            if (!child.gameObject.CompareTag("ButtonPriority"))
            {
                Destroy(child.gameObject);
            }
        }

        // Get all save files
        #if UNITY_WEBGL && !UNITY_EDITOR
    string saveFolderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
    string saveFolderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif
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

            string townName = GetTownNameFromJson(saveFile);

            GameObject saveButton = Instantiate(saveButtonPrefab, saveListParent);
            saveButton.transform.SetAsFirstSibling();
            saveButton.transform.localPosition -= new Vector3(0,(saveFiles.Length -1 ) * 100 - index * 100, 0);
            index++;

            TMP_Text textComponent = saveButton.transform.Find("Text (TMP)")?.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                string formattedDate = saveDate.Replace("-", "/").Replace("_", " - ");
                int lastDashIndex = formattedDate.LastIndexOf('-');
                if (lastDashIndex != -1 && lastDashIndex + 2 < formattedDate.Length)
                {
                    string timePart = formattedDate.Substring(lastDashIndex + 2).Replace("/", "");
                    if (timePart.Length >= 4)
                    {
                        timePart = timePart.Insert(2, "h");
                        timePart = timePart.Insert(5, "m");
                        formattedDate = formattedDate.Substring(0, lastDashIndex + 2) + timePart;
                    }
                }

                textComponent.text = $"{townName} - {formattedDate}";
            }


            var buttons = saveButton.GetComponentsInChildren<Button>();
            if (buttons.Length >= 2)
            {
                if (SceneManager.GetActiveScene().name == "Menue")
                {
                    buttons[0].onClick.AddListener(() =>
                    {
                        Materials.instance.isLoad = true;
                        Materials.instance.tutorial = false;
                        LoadManager.instance.saveDate = saveDate;
                        LoadManager.instance.resumeLoad = true;
        StartCoroutine(LoadSceneAsync("SampleScene"));
                    });
                }
                else
                {
                    buttons[0].onClick.AddListener(() => gameDataSaver.LoadData(saveDate));
                }

                buttons[1].onClick.AddListener(() => DeleteSave(saveFile, saveButton));
            }
        }
    }

    private string GetTownNameFromJson(string filePath)
    {
        if (!File.Exists(filePath)) return "Ville inconnue";

        try
        {
            string jsonContent = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(jsonContent);
            return !string.IsNullOrEmpty(data.townName) ? data.townName : "Ville inconnue";
        }
        catch
        {
            return "Ville inconnue";
        }
    }


    private void DeleteSave(string filePath, GameObject saveButton)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        Destroy(saveButton);
        Invoke(nameof(LoadSaveFiles), 0.1f);
    }
}


