using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Materials : MonoBehaviour
{
    public static Materials instance;

    public bool tutorial = true;
    public bool tutorialStep = false;
    public bool researchCentr = false;
    public bool explored = false;
    public bool event3Active = false;

    public string townName;
    public int mat_0 = 0;
    public int mat_1 = 0;
    public int mat_2 = 0;
    public int price = 0;
    public float bar_0 = 0.5f;
    public float bar_1 = 0.5f;
    public float bar_2 = 0.5f;
    public bool isLoad = false;

    public int sessionWood = 0;
    public int sessionStone = 0;
    public int sessionIron = 0;

    public bool canMove = true;
    public bool textDone = false;
    public bool tutoToggle = false;
    public GameObject victoryScreen;
    private bool victory = false;

    public GameObject errorIndicator;
    public Text errorText;
    public Button ResBut;


    public bool menuFirst = true;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            if (errorIndicator != null)
            {
                errorIndicator.SetActive(true);
            }
            if (errorText != null)
            {
                errorText.text = "Error: " + logString;
            }
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances de Materials détectées.");
        }


        if (menuFirst == true && LoadManager.instance == null && SceneManager.GetActiveScene().name != "Exploration_main")
        {
            SceneManager.LoadScene("Menue");
            menuFirst = false;
        }
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            ResBut.interactable = false;
        }

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (obj.CompareTag("Victory"))
            {
                victoryScreen = obj;
                break;
            }
        }
    }

    public void ReseachButton(bool act)
    {
        if (ResBut == null)
        {
            // Cherche le GameObject avec le tag "FishGallery"
            GameObject fishGalleryObject = GameObject.FindGameObjectWithTag("FishGallery");

            if (fishGalleryObject != null)
            {
                // Récupère le bouton s'il existe
                ResBut = fishGalleryObject.GetComponent<Button>();
            }
            else
            {
                // Affiche un message d'erreur si l'objet n'a pas été trouvé
                Debug.LogError("FishGallery GameObject not found in the scene.");
                return;
            }
        }

        // Si ResBut est trouvé, met à jour l'état interactable du bouton
        if (ResBut != null)
        {
            ResBut.interactable = act;
        }
    }


    void Start()
    {
        if (GameDataSaver.instance != null)
        {
            mat_0 = GameDataSaver.instance.mat_0;
            mat_1 = GameDataSaver.instance.mat_1;
            mat_2 = GameDataSaver.instance.mat_2;
            price = GameDataSaver.instance.price;

            ResetSessionCounts();
        }
        else
        {
            Debug.LogWarning("⚠️ GameDataSaver.instance est null dans Materials.Start()");
        }

        if (isLoad)
        {
            tutorial = false;

        }

    }

    void Update()
    {

        bar_0 = Mathf.Clamp(bar_0, 0f, 0.99f);
        bar_1 = Mathf.Clamp(bar_1, 0f, 0.99f);
        bar_2 = Mathf.Clamp(bar_2, 0f, 0.99f);

        if (bar_2 == 0.0f && victory == false)
        {
            if (victoryScreen != null)
            {
                victoryScreen.SetActive(true);
                victory = true;
            }
            else
            {
                Debug.LogError("❌ Impossible d'afficher VictoryScreen car il est null !");
            }
        }
    }

    public void AddWood(int amount)
    {
        mat_1 += amount;
        sessionWood += amount;

        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_1 = mat_1;
        }
    }

    public void AddStone(int amount)
    {
        mat_0 += amount;
        sessionStone += amount;

        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_0 = mat_0;
        }
    }

    public void AddIron(int amount)
    {
        mat_2 += amount;
        sessionIron += amount;

        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_2 = mat_2;
        }
    }

    public void ResetSessionCounts()
    {
        sessionWood = 0;
        sessionStone = 0;
        sessionIron = 0;
    }
}
