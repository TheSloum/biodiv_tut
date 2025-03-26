using UnityEngine;
using UnityEngine.UI;

public class BtnSaver : MonoBehaviour
{
    private GameDataSaver savescript;

    void Awake()
    {
        savescript = Materials.instance.GetComponent<GameDataSaver>();

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            Debug.Log("BtnSaver: Button found");
            btn.onClick.AddListener(SaveData);
        }
    }

    void SaveData()
    {
        if (savescript != null)
        {
            Debug.Log("BtnSaver: Button founssssssd");
            savescript.SaveData();
        }
        else
        {
            Debug.LogWarning("GameDataSaver non trouvé !");
        }
    }
}
