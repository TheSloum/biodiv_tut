using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameDataSaver : MonoBehaviour
{
    public List<Fishes> fishUnlockData;
    public List<Building> buildUnlockData;
    public List<GameObject> builderData;

    private void Start()
    {
        builderData = new List<GameObject>(GameObject.FindGameObjectsWithTag("Building"));
        SaveData();
    }

    public void SaveData()
    {
        GameData gameData = new GameData
        {
            fishDataList = new List<FishData>(),
            buildingDataList = new List<BuildingData>(),
            builderDataList = new List<BuilderData>()
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

        string json = JsonUtility.ToJson(gameData, true);
        string path = Path.Combine(Application.dataPath, "Sauvegardes/GameData.json");
        File.WriteAllText(path, json);
        Debug.Log("Game data saved to: " + path);
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
}