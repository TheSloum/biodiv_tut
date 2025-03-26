using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using TMPro;

using System.Collections;

public class Materials : MonoBehaviour
{
    private Queue<KeyCode> keySequence = new Queue<KeyCode>();
    private KeyCode[] cheatCode = { KeyCode.G, KeyCode.U, KeyCode.C, KeyCode.C, KeyCode.I };

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


    public bool dys = false;
    public bool fulls = false;

    public int resX;

    public bool menuFirst = true;
    public bool cinematique = true;
    public GameObject loadingObject;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
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
    private void ActivateCheat()
    {
        mat_0 = 99999;
        mat_1 = 99999;
        mat_2 = 99999;
        price = 99999;
        bar_0 = 0.5f;
        bar_1 = 0.5f;
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
    public void ResetState()
    {
        tutorial = true;
        tutorialStep = false;
        researchCentr = false;
        explored = false;
        event3Active = false;

        townName = "";
        mat_0 = 0;
        mat_1 = 0;
        mat_2 = 0;
        price = 500;
        bar_0 = 0.5f;
        bar_1 = 0.5f;
        bar_2 = 0.95f;
        isLoad = false;

        sessionWood = 0;
        sessionStone = 0;
        sessionIron = 0;

        canMove = true;
        textDone = false;
        tutoToggle = false;
        victory = false;

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        if (errorIndicator != null)
        {
            errorIndicator.SetActive(false);
        }

        if (errorText != null)
        {
            errorText.text = "";
        }

        menuFirst = true;

        Debug.Log("✅ Game Reset Done !");
    }

    private void Awake()
    {
        loadingObject = GameObject.Find("loadingScreen");
        if (E_GameManager.instance != null) return;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (menuFirst == true)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances de Materials détectées.");
        }



        if (menuFirst && LoadManager.instance == null && SceneManager.GetActiveScene().name != "Exploration_main" && isLoad == false)
        {
            Debug.Log(E_GameManager.instance != null);
            StartCoroutine(LoadSceneAsync("Menue"));
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

        loadingObject = GameObject.Find("loadingScreen");
    }

    public void ReseachButton(bool act)
    {
        if (ResBut == null)
        {
            GameObject fishGalleryObject = GameObject.FindGameObjectWithTag("FishGallery");

            if (fishGalleryObject != null)
            {
                ResBut = fishGalleryObject.GetComponent<Button>();
            }
            else
            {
                return;
            }
        }

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
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in cheatCode)
            {
                if (Input.GetKeyDown(key))
                {
                    keySequence.Enqueue(key);
                    if (keySequence.Count > cheatCode.Length)
                    {
                        keySequence.Dequeue();
                    }
                    break;
                }
            }

            if (keySequence.Count == cheatCode.Length && new List<KeyCode>(keySequence).ToArray().SequenceEqual(cheatCode))
            {
                ActivateCheat();
                keySequence.Clear();
            }
        }

        bar_0 = Mathf.Clamp(bar_0, 0f, 0.99f);
        bar_1 = Mathf.Clamp(bar_1, 0f, 0.99f);
        bar_2 = Mathf.Clamp(bar_2, 0f, 0.99f);

        string element1 = "";
        string element2 = "";
        bool shouldShowRevive = false;

        if (bar_0 == 0.0f && bar_1 == 0.0f)
        {
            bar_0 = 0.3f;
            bar_1 = 0.3f;
            element1 = "de qualité de vie";
            element2 = "d'energie";
            shouldShowRevive = true;
        }

        if (bar_0 == 0.0f && price == 0)
        {
            bar_0 = 0.3f;
            price = 500;
            element1 = "de qualité de vie";
            element2 = "d'argent";
            shouldShowRevive = true;
        }

        if (bar_1 == 0.0f && price == 0)
        {
            bar_1 = 0.3f;
            price = 500;
            element1 = "d'energie";
            element2 = "d'argent";
            shouldShowRevive = true;
        }

        if (shouldShowRevive)
        {
            GameObject reviveEndObject = Resources.FindObjectsOfTypeAll<GameObject>()
     .FirstOrDefault(obj => obj.CompareTag("reviveend"));

            if (reviveEndObject != null)
            {
                reviveEndObject.SetActive(true); // 🔥 Active l'objet s'il est désactivé

                Transform textTransform = reviveEndObject.transform.Find("SofLockText");

                if (textTransform != null)
                {
                    TextMeshProUGUI childText = textTransform.GetComponent<TextMeshProUGUI>();

                    if (childText != null)
                    {
                        childText.text = $"Mince, tu es bloqué ! <u>{townName}</u> est à court <b>{element1}</b> et <b>{element2}</b>. Impossible de faire redémarrer tes bâtiments. Tu peux <b>recommencer</b> une partie, <b>reprendre</b> ta dernière sauvegarde ou ajouter un peu de <b>ressources</b> manquantes.";
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ Le GameObject 'SofLockText' existe mais ne contient pas de composant TextMeshProUGUI.");
                    }
                }
                else
                {
                    Debug.LogWarning("⚠️ Aucun GameObject nommé 'SofLockText' trouvé dans 'reviveend'.");
                }
            }
            else
            {
                Debug.LogWarning("⚠️ Aucun objet avec le tag 'reviveend' trouvé dans la scène.");
            }

        }

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
