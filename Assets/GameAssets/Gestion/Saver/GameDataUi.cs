using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameDataUi : MonoBehaviour
{
    public TextMeshProUGUI saveText; // Reference to the TextMeshPro component
    public Button loadButton; // Reference to the Load button
    public Button deleteButton; // Reference to the Delete button

    public void Initialize(string saveDate, GameDataSaver gameDataSaver)
    {
        // Update the text to show the save date
        saveText.text = $"Sauvegarde du {saveDate}";

        // Set up button listeners with the corresponding save date
        loadButton.onClick.AddListener(() => gameDataSaver.LoadData(saveDate));
        deleteButton.onClick.AddListener(() => gameDataSaver.DelData(saveDate));
    }
}
