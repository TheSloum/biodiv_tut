using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonReasign : MonoBehaviour
{
    private Button saveButton;

    void Start()
    {
        saveButton = GetComponent<Button>();

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGameData);
        }
    }

    void SaveGameData()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager != null)
        {
            GameDataSaver gameDataSaver = gameManager.GetComponent<GameDataSaver>();
            if (gameDataSaver != null)
            {
                gameDataSaver.SaveData();
            }
            else
            {
                Debug.LogError("GameDataSaver component not found on GameManager.");
            }
        }
        else
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }
}
