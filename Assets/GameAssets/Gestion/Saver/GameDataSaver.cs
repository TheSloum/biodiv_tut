using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI; // Ajoute ceci en haut

using System.Collections;


public class GameDataSaver : MonoBehaviour
{
    public bool isSavingCompleted = false;
    public bool SaveAndLoadScene = false;
    public static GameDataSaver instance { get; private set; }
    public Button trasiButton;
    public List<Fishes> fishUnlockData;
    public List<Building> buildUnlockData;
    public List<GameObject> builderData;

    public Sprite baseSprite;
    public Sprite researchSprite;

    public bool reseachHere;

    public int mat_0 = 0;
    public int mat_1 = 0;
    public int mat_2 = 0;
    public int price = 0;

    public GameObject loadingObject;
    private void Awake()
    {


        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
#if UNITY_WEBGL && !UNITY_EDITOR
    string saveFolderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
        string saveFolderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif

        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }


    }



    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            trasiButton = GameObject.Find("Trasi")?.GetComponent<Button>();
            if (trasiButton != null)
            {
                trasiButton.onClick.AddListener(() => SaveData());
            }
            else
            {
                Debug.LogWarning("⚠️ Bouton Trasi introuvable !");
            }

            builderData.Clear();
            Builder[] builders = FindObjectsOfType<Builder>();
            foreach (Builder builder in builders)
            {
                builderData.Add(builder.gameObject);
            }

            if ((Materials.instance.isLoad && Materials.instance.explored) || (LoadManager.instance != null && LoadManager.instance.resumeLoad))
            {
                Materials.instance.isLoad = true;
                LoadLatestSaveData();
                Materials.instance.explored = false;
            }

        }

        loadingObject = GameObject.Find("loadingScreen");
    }





    public void LoadLatestSaveData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    string saveFolderPath = Path.Combine(Application.persistentDataPath, "Sauvegardes");
#else
        string saveFolderPath = Path.Combine(Application.dataPath, "Sauvegardes");
#endif

        if (!Directory.Exists(saveFolderPath))
        {
            return;
        }

        var saveFiles = Directory.GetFiles(saveFolderPath, "GameData_*.json");
        if (saveFiles.Length == 0)
        {
            return;
        }

        string latestFile = saveFiles.OrderByDescending(File.GetLastWriteTime).First();
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(latestFile);
        string datePart = fileNameWithoutExtension.Replace("GameData_", "");

        LoadDataAfterExplore(datePart);

    }

    public void SaveData()
    {
        isSavingCompleted = false;
        SaveAndLoadScene = false;
        if (Materials.instance == null)
        {
            Debug.LogWarning("Materials.instance est null, annulation de SaveData().");
            return;
        }

        GameData gameData = new GameData
        {
            fishDataList = new List<FishData>(),
            buildingDataList = new List<BuildingData>(),
            builderDataList = new List<BuilderData>(),
            mat_0 = Materials.instance.mat_0,
            mat_1 = Materials.instance.mat_1,
            mat_2 = Materials.instance.mat_2,
            bar_0 = Materials.instance.bar_0,
            bar_1 = Materials.instance.bar_1,
            bar_2 = Materials.instance.bar_2,
            price = Materials.instance.price,
            townName = Materials.instance.townName
        };

        // Fish data
        foreach (var fish in fishUnlockData)
        {
            gameData.fishDataList.Add(new FishData { is_unlocked = fish.is_unlocked });
        }

        // Building data
        foreach (var building in buildUnlockData)
        {
            gameData.buildingDataList.Add(new BuildingData { unlocked = building.unlocked });
        }

        // Builder data
        // Avant on faisait directement un GetComponent, maintenant on vérifie.
        // Builder data


        if (builderData != null)
        {
            foreach (var builderObject in builderData)
            {
                if (builderObject == null)
                {
                    Debug.LogWarning("Un builder est null dans builderData !");
                    continue;
                }

                Builder builderComponent = builderObject.GetComponent<Builder>();
                if (builderComponent == null)
                {
                    Debug.LogWarning($"L'objet {builderObject.name} n'a pas de composant Builder !");
                    continue;
                }

                BuilderData newBuilderData = new BuilderData
                {
                    level0 = builderComponent.level0,
                    level1 = builderComponent.level1,
                    level2 = builderComponent.level2,
                    running = builderComponent.running,
                    buildState = builderComponent.buildState
                };

                gameData.builderDataList.Add(newBuilderData);
            }

        }


        // Sauvegarde en fichier

        string fileName;

        fileName = $"GameData_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.json";


#if UNITY_WEBGL && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, "Sauvegardes", fileName);
#else
        string path = Path.Combine(Application.dataPath, "Sauvegardes", fileName);
#endif
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(path, json);
        SaveAndLoadScene = true;
    }

    public void LoadData(string dataDate)
    {
        if (SceneManager.GetActiveScene().name == "Menue")
        {

            Materials.instance.isLoad = true;
            Materials.instance.explored = false;
            StartCoroutine(LoadSceneAsync("SampleScene"));
            Materials.instance.menuFirst = false;
        }
        Materials.instance.isLoad = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, $"Sauvegardes/GameData_{dataDate}.json");
#else
        string path = Path.Combine(Application.dataPath, $"Sauvegardes/GameData_{dataDate}.json");
#endif
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            for (int i = 0; i < fishUnlockData.Count && i < gameData.fishDataList.Count; i++)
            {
                fishUnlockData[i].is_unlocked = gameData.fishDataList[i].is_unlocked;
                if (fishUnlockData[i].is_unlocked)
                {
                    fishUnlockData[i].UnlockFish();
                }
            }

            for (int i = 0; i < buildUnlockData.Count && i < gameData.buildingDataList.Count; i++)
            {
                buildUnlockData[i].unlocked = gameData.buildingDataList[i].unlocked;
            }

            // Recharger builder
            for (int i = 0; i < builderData.Count && i < gameData.builderDataList.Count; i++)
            {
                GameObject bObj = builderData[i];
                if (bObj == null) continue;
                Builder builderComponent = bObj.GetComponent<Builder>();
                SpriteRenderer spriterenderer = bObj.GetComponent<SpriteRenderer>();
                int buildState = gameData.builderDataList[i].buildState;

                if (builderComponent != null && spriterenderer != null)
                {
                    if (buildState == 50)
                    {
                        builderComponent.editing = true;
                        builderComponent.OnDestroyClicked();
                        builderComponent.editing = false;
                        builderComponent.buildState = 50;
                        reseachHere = true;
                        builderComponent.running = true;

                        Materials.instance.researchCentr = false;
                        Materials.instance.ReseachButton(true);

                        spriterenderer.sprite = researchSprite;
                    }
                    else if (buildState >= 0 && buildState < buildUnlockData.Count)
                    {
                        builderComponent.editing = true;
                        builderComponent.OnDestroyClicked();
                        builderComponent.editing = false;
                        builderComponent.progress = 0f;
                        builderComponent.level0 = gameData.builderDataList[i].level0;
                        builderComponent.level1 = gameData.builderDataList[i].level1;
                        builderComponent.level2 = gameData.builderDataList[i].level2;
                        builderComponent.buildState = gameData.builderDataList[i].buildState;
                        builderComponent.price_cycle = buildUnlockData[gameData.builderDataList[i].buildState].price_cycle;
                        builderComponent.mat_0_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_0;
                        builderComponent.mat_1_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_1;
                        builderComponent.mat_2_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_2;
                        builderComponent.bar_2_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_2_cycle;
                        builderComponent.bar_1_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_1_cycle;
                        builderComponent.bar_0_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_0_cycle;
                        builderComponent.running = true;
                        
                        if(gameData.builderDataList[i].running == false){
                            builderComponent.toFloat = true;
                        }


                        if (builderComponent.buildState == 0)
                        {
                            spriterenderer.sprite = builderComponent.baseSprite;
                        }
                        else
                        {
                            builderComponent.cycleBar.transform.localPosition = new Vector3(0, 83, 0);
                            builderComponent.barFs.sortingOrder = -2;
                            builderComponent.barBs.sortingOrder = -3;
                            spriterenderer.sprite = buildUnlockData[gameData.builderDataList[i].buildState].buildSprite;
                        }

                        builderComponent.cycleDuration = buildUnlockData[gameData.builderDataList[i].buildState].time;
                        if (builderComponent.buildState > 0 && builderComponent.running)
                        {
                            builderComponent.StartCycle();
                        }
                    }
                }
            }

            Materials.instance.mat_0 = gameData.mat_0;
            Materials.instance.mat_1 = gameData.mat_1;
            Materials.instance.mat_2 = gameData.mat_2;
            Materials.instance.bar_0 = gameData.bar_0;
            Materials.instance.bar_1 = gameData.bar_1;
            Materials.instance.bar_2 = gameData.bar_2;
            Materials.instance.price = gameData.price;
            Materials.instance.townName = gameData.townName;
            Materials.instance.isLoad = true;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée à: " + path);
        }
        isSavingCompleted = true;

    }


    public void LoadDataAfterExplore(string dataDate)
    {
        if (LoadManager.instance.saveDate != null && !Materials.instance.explored)
        {
            dataDate = LoadManager.instance.saveDate;
            LoadManager.instance.saveDate = null;
        }
        Materials.instance.isLoad = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, $"Sauvegardes/GameData_{dataDate}.json");
#else
        string path = Path.Combine(Application.dataPath, $"Sauvegardes/GameData_{dataDate}.json");
#endif


        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);
            for (int i = 0; i < fishUnlockData.Count && i < gameData.fishDataList.Count; i++)
            {
                fishUnlockData[i].is_unlocked = gameData.fishDataList[i].is_unlocked;
                if (E_GameManager.instance != null)
                {
                    if (E_GameManager.instance.fishUnlockData[i] == true)
                    {
                        fishUnlockData[i].is_unlocked = true;
                    }
                }
            }

            for (int i = 0; i < buildUnlockData.Count && i < gameData.buildingDataList.Count; i++)
            {
                buildUnlockData[i].unlocked = gameData.buildingDataList[i].unlocked;
            }


            for (int i = 0; i < builderData.Count && i < gameData.builderDataList.Count; i++)
            {
                GameObject bObj = builderData[i];
                if (bObj == null) continue;
                Builder builderComponent = bObj.GetComponent<Builder>();
                SpriteRenderer spriterenderer = bObj.GetComponent<SpriteRenderer>();

                int buildState = gameData.builderDataList[i].buildState;

                if (builderComponent != null && spriterenderer != null)
                {
                    if (buildState == 50)
                    {
                        builderComponent.editing = true;
                        builderComponent.OnDestroyClicked();
                        builderComponent.editing = false;
                        builderComponent.buildState = 50;
                        builderComponent.running = true;
                        reseachHere = true;

                        spriterenderer.sprite = researchSprite;

                        Materials.instance.researchCentr = false;
                        Materials.instance.ReseachButton(true);
                    }
                    else if (buildState >= 0 && buildState < buildUnlockData.Count)
                    {

                        builderComponent.editing = true;
                        builderComponent.OnDestroyClicked();
                        builderComponent.editing = false;
                        builderComponent.progress = 0f;
                        builderComponent.level0 = gameData.builderDataList[i].level0;
                        builderComponent.level1 = gameData.builderDataList[i].level1;
                        builderComponent.level2 = gameData.builderDataList[i].level2;
                        builderComponent.buildState = gameData.builderDataList[i].buildState;
                        builderComponent.price_cycle = buildUnlockData[gameData.builderDataList[i].buildState].price_cycle;
                        builderComponent.mat_0_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_0;
                        builderComponent.mat_1_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_1;
                        builderComponent.mat_2_cycle = buildUnlockData[gameData.builderDataList[i].buildState].cons_mat_2;
                        builderComponent.bar_2_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_2_cycle;
                        builderComponent.bar_1_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_1_cycle;
                        builderComponent.bar_0_cycle = buildUnlockData[gameData.builderDataList[i].buildState].bar_0_cycle;
                            builderComponent.running = true;
                        if(gameData.builderDataList[i].running == false){
                            builderComponent.toFloat = true;
                        }




                        if (builderComponent.buildState == 0)
                        {
                            spriterenderer.sprite = builderComponent.baseSprite;
                        }
                        else
                        {
                            builderComponent.cycleBar.transform.localPosition = new Vector3(0, 83, 0);
                            builderComponent.barFs.sortingOrder = -2;
                            builderComponent.barBs.sortingOrder = -3;
                            spriterenderer.sprite = buildUnlockData[gameData.builderDataList[i].buildState].buildSprite;
                        }

                        builderComponent.cycleDuration = buildUnlockData[gameData.builderDataList[i].buildState].time;
                        if (builderComponent.buildState > 0 && builderComponent.running)
                        {
                            builderComponent.StartCycle();
                        }
                    }
                }
            }
            Debug.Log(gameData.mat_0);
            if (!Materials.instance.explored)
            {
                Materials.instance.mat_0 = gameData.mat_0;
                Materials.instance.mat_1 = gameData.mat_1;
                Materials.instance.mat_2 = gameData.mat_2;
            Materials.instance.bar_0 = gameData.bar_0;
            Materials.instance.bar_1 = gameData.bar_1;
            Materials.instance.bar_2 = gameData.bar_2;
                Materials.instance.price = gameData.price;
            }
            Materials.instance.townName = gameData.townName;
            Materials.instance.isLoad = true;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée à: " + path);
        }
        isSavingCompleted = true;

        if (!LoadManager.instance.resumeLoad)
        {
            File.Delete(path);
        }
        else
        {
            Debug.Log("ResumeDone");
            LoadManager.instance.resumeLoad = false;

            Destroy(LoadManager.instance.gameObject);
        }

    }


    public void DelData(string dataDate)
    {


#if UNITY_WEBGL && !UNITY_EDITOR
        string path = Path.Combine(Application.persistentDataPath, $"Sauvegardes/GameData_{dataDate}.json");
#else
        string path = Path.Combine(Application.dataPath, $"Sauvegardes/GameData_{dataDate}.json");
#endif
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}

[System.Serializable]
public class FishData
{
    public bool is_unlocked;
}

[System.Serializable]
public class BuildingData
{
    public bool unlocked;
}

[System.Serializable]
public class BuilderData
{
    public int level0;
    public int level1;
    public int level2;
    public bool running;
    public int buildState;

    public int cons_mat_0;

    public int cons_mat_1;

    public int cons_mat_2;

    public float bar_0_cycle;

    public float bar_1_cycle;
    public float bar_2_cycle;
    public int price_cycle = 0;
}

[System.Serializable]
public class GameData
{
    public List<FishData> fishDataList;
    public List<BuildingData> buildingDataList;
    public List<BuilderData> builderDataList;

    public int mat_0;
    public int mat_1;
    public int mat_2;
    public float bar_0;
    public float bar_1;
    public float bar_2;
    public int price;

    public string townName;
}
