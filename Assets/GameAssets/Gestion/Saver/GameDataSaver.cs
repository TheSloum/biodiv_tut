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

    public int mat_0 = 500; 
    public int mat_1 = 500; 
    public int mat_2 = 500;
    public int price = 500;

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

    public void SaveData()
    {
        // Vérifications avant de faire quoi que ce soit
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
        if (builderData != null)
        {
            foreach (var builderObject in builderData)
            {
                if (builderObject == null) continue;
                Builder builderComponent = builderObject.GetComponent<Builder>();
                if (builderComponent == null) continue;

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

        // Sauvegarde en fichier
        string fileName = $"GameData_{DateTime.Now:yyyy-MM-dd_HH-mm}.json";
        string path = Path.Combine(Application.dataPath, "Sauvegardes", fileName);
        string json = JsonUtility.ToJson(gameData, true);
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

            // Recharger fishes
            for (int i = 0; i < fishUnlockData.Count && i < gameData.fishDataList.Count; i++)
            {
                fishUnlockData[i].is_unlocked = gameData.fishDataList[i].is_unlocked;
            }

            // Recharger buildings
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
                if (builderComponent != null && spriterenderer != null && buildUnlockData.Count > gameData.builderDataList[i].buildState)
                {
                    builderComponent.level0 = gameData.builderDataList[i].level0;
                    builderComponent.level1 = gameData.builderDataList[i].level1;
                    builderComponent.level2 = gameData.builderDataList[i].level2;
                    builderComponent.running = gameData.builderDataList[i].running;
                    builderComponent.buildState = gameData.builderDataList[i].buildState;

                    if (builderComponent.buildState == 0)
                    {
                        spriterenderer.sprite = baseSprite;
                    }
                    else
                    {
                        spriterenderer.sprite = buildUnlockData[gameData.builderDataList[i].buildState].buildSprite;
                    }

                    builderComponent.cycleDuration = buildUnlockData[gameData.builderDataList[i].buildState].time;
                    if (builderComponent.buildState > 0 && builderComponent.running)
                    {
                        builderComponent.StartCycle();
                    }
                }
            }

            Materials.instance.mat_0 = gameData.mat_0;
            Materials.instance.mat_1 = gameData.mat_1;
            Materials.instance.mat_2 = gameData.mat_2;
            Materials.instance.price = gameData.price;
            Materials.instance.townName = gameData.townName;
            Materials.instance.isLoad = true;

            Debug.Log("Jeu chargé avec succès !");
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

    public int mat_0;
    public int mat_1;
    public int mat_2;
    public int price;

    public string townName;
}
