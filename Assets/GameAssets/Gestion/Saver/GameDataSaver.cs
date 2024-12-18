using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class GameDataSaver : MonoBehaviour
{
    public static GameDataSaver instance { get; private set; }

    public List<Fishes> fishUnlockData;
    public List<Building> buildUnlockData;
    public List<GameObject> builderData;

    public Sprite baseSprite;
    
    // Matériaux
    public int mat_0 = 500; // Bois
    public int mat_1 = 500; // Pierre
    public int mat_2 = 500; // Fer
    public int price = 500; // MONEYYYY

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
    }

    private void Start()
    {
        builderData = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
        // Optionnel : SaveData(); // Sauvegarder immédiatement après le chargement (à utiliser si nécessaire)
    }

    

    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S))
        {
            SaveData();
//save
        }
    }

    public void SaveData()
    {
        GameData gameData = new GameData
        {
            fishDataList = new List<FishData>(),
            buildingDataList = new List<BuildingData>(),
            builderDataList = new List<BuilderData>(),
            mat_0 = Materials.instance.mat_0,
            mat_1 = Materials.instance.mat_1,
            mat_2 = Materials.instance.mat_2,
            price = Materials.instance.price
        };

        foreach (var fish in fishUnlockData)
        {
            gameData.fishDataList.Add(new FishData { is_unlocked = fish.is_unlocked });
        }

        foreach (var building in buildUnlockData)
        {
            gameData.buildingDataList.Add(new BuildingData { unlocked = building.unlocked });
        }

        foreach (var builder in builderData)
        {
            Builder builderComponent = builder.GetComponent<Builder>();

            if (builderComponent != null)
            {
                gameData.builderDataList.Add(new BuilderData
                {
                    level0 = builderComponent.level0,
                    level1 = builderComponent.level1,
                    level2 = builderComponent.level2,
                    running = builderComponent.running,
                    buildState = builderComponent.buildState
                });
            }
        }

        // Sauvegarder les matériaux
        gameData.mat_0 = Materials.instance.mat_0;
        gameData.mat_1 = Materials.instance.mat_1;
        gameData.mat_2 = Materials.instance.mat_2;
        gameData.price = Materials.instance.price;
        gameData.townName = Materials.instance.townName;

        string json = JsonUtility.ToJson(gameData, true);
        string fileName = $"GameData_{DateTime.Now:yyyy-MM-dd_HH-mm}.json";
        string path = Path.Combine(Application.dataPath, "Sauvegardes", fileName);
        File.WriteAllText(path, json);
        Debug.Log("Game data saved to: " + path);
    }

    public void LoadData(string dataDate)
    {
        string path = Path.Combine(Application.dataPath, $"Sauvegardes/GameData_{dataDate}.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            // Charger les données des poissons
            for (int i = 0; i < fishUnlockData.Count && i < gameData.fishDataList.Count; i++)
            {
                fishUnlockData[i].is_unlocked = gameData.fishDataList[i].is_unlocked;
            }

            // Charger les données des bâtiments
            for (int i = 0; i < buildUnlockData.Count && i < gameData.buildingDataList.Count; i++)
            {
                buildUnlockData[i].unlocked = gameData.buildingDataList[i].unlocked;
            }

            // Charger les données des builders
            for (int i = 0; i < builderData.Count && i < gameData.builderDataList.Count; i++)
            {
                Builder builderComponent = builderData[i].GetComponent<Builder>();
                SpriteRenderer spriterenderer = builderData[i].GetComponent<SpriteRenderer>();
                if (builderComponent != null)
                {
                    builderComponent.level0 = gameData.builderDataList[i].level0;
                    builderComponent.level1 = gameData.builderDataList[i].level1;
                    builderComponent.level2 = gameData.builderDataList[i].level2;
                    builderComponent.running = gameData.builderDataList[i].running;
                    builderComponent.buildState = gameData.builderDataList[i].buildState;
                    if(gameData.builderDataList[i].buildState == 0){
                        spriterenderer.sprite = baseSprite;
                    } else{
                    spriterenderer.sprite = buildUnlockData[gameData.builderDataList[i].buildState].buildSprite;
                    }
                    builderComponent.cycleDuration = buildUnlockData[gameData.builderDataList[i].buildState].time;
                    if (builderComponent.buildState > 0 && builderComponent.running)
                    {
                        builderComponent.StartCycle();
                    }
                }
            }

            // Charger les matériaux
            Materials.instance.mat_0 = gameData.mat_0;
            Materials.instance.mat_1 = gameData.mat_1;
            Materials.instance.mat_2 = gameData.mat_2;
            Materials.instance.price = gameData.price;

            Materials.instance.townName = gameData.townName;

            Materials.instance.isLoad = true;
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde trouvée à: " + path);
        }
    }

    public void DelData(string dataDate)
    {
        string path = Path.Combine(Application.dataPath, $"Sauvegardes/GameData_{dataDate}.json");
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
}

[System.Serializable]
public class GameData
{
    public List<FishData> fishDataList;
    public List<BuildingData> buildingDataList;
    public List<BuilderData> builderDataList;

    // Matériaux
    public int mat_0; // Bois
    public int mat_1; // Pierre
    public int mat_2; // Fer
    public int price; // Fer

    public string townName;
}
