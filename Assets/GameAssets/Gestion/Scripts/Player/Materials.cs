using UnityEngine;

public class Materials : MonoBehaviour
{
    public static Materials instance;


    public bool tutorial = true;
    public bool tutorialStep = false;
    public bool researchCentr = false;

    public bool explored = false;

    public string townName;
    public int mat_0 = 500; 
    public int mat_1 = 500;
    public int mat_2 = 500; 
    public int price = 500; 
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Materials.instance initialisé.");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances de Materials détectées.");
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
            Debug.Log($"Materials chargés: Bois = {mat_0}, Pierre = {mat_1}, Fer = {mat_2}, Price = {price}");
            ResetSessionCounts();
        }
        else
        {
            Debug.LogWarning("GameDataSaver.instance est null dans Materials.Start()");
        }
        if(isLoad){
            tutorial = false;
        }
    }

    void Update()
    {
        bar_0 = Mathf.Clamp(bar_0, 0f, 0.99f);
        bar_1 = Mathf.Clamp(bar_1, 0f, 0.99f);
        bar_2 = Mathf.Clamp(bar_2, 0f, 0.99f);
    }

    public void AddWood(int amount)
    {
        mat_0 += amount;
        sessionWood += amount;

        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_0 = mat_0;
        }
        Debug.Log(mat_0);
    }

    public void AddStone(int amount)
    {
        mat_1 += amount;
        sessionStone += amount;

        if (GameDataSaver.instance != null)
        {
            GameDataSaver.instance.mat_1 = mat_1;
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
        Debug.Log("Compteurs session reset.");
    }
}
