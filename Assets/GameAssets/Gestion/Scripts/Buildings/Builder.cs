using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class Builder : MonoBehaviour
{
    
    [SerializeField] private List<Building> buildings = new List<Building>();
    [SerializeField] private GameObject buttonPrefab; 
    [SerializeField] private Transform buttonPanel; 

    [SerializeField] private GameObject buildingMenu; 
    [SerializeField] private GameObject manageMenu; 
    [SerializeField] private GameObject closeMenu; 
    [SerializeField] private GameObject closeMenu2; 
    [SerializeField] private GameObject destroyButton; 
    [SerializeField] private GameObject upgradeButton1;
    [SerializeField] private GameObject upgradeButton2;
    [SerializeField] private GameObject upgradeButton3;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private Sprite baseSprite;

    private SpriteRenderer spriteRenderer; 

    public int buildState = 0;
    private int buildID = 0;

    private Button closeMenuButton;
    private Button closeMenuButton2;
    private Button destroyB;
    private Button upgrade1;
    private Button upgrade2;
    private Button upgrade3;
    private Button pause;



    public bool running = false;
    private float cycleDuration = 0f;
    private Coroutine cycleCoroutine;
    private int mat_0_cycle = 0;
    private int mat_1_cycle = 0;
    private int mat_2_cycle = 0;
    private float bar_0_cycle = 0f;
    private float bar_1_cycle = 0f;
    private float bar_2_cycle = 0f;
    private int price_cycle = 0;
    public int level0 = 0;
    public int level1 = 0;
    public int level2 = 0;

    private float level1ScaleUp = 0.2f;
    private float level2ScaleUp = 0.4f;
    private float level3ScaleUp = 0.6f;


    private bool toFloat = false;

    public Animator cycleAnim;

    private float progress;


    public float outsideTime = 0;

    public bool editing = false;




public GameObject prefabToSpawn;           // The prefab to spawn
    public Vector3 spawnPosition;              // The base position to spawn prefabs at
    public float positionOffset = 1f;          // The offset position for each new spawn
    public float timeOffset = 0.2f;            // Delay between each spawn
    public float fadeDuration = 1f;            // Duration for fading out
    public float moveSpeed = 1f;               // Speed at which the object moves upward

    private float[] numbers = new float[4];    // Array to store numbers to display
    public Sprite[] sprites = new Sprite[4];




    private void Start(){
        closeMenuButton = closeMenu.GetComponent<Button>();
        closeMenuButton2 = closeMenu2.GetComponent<Button>();

        closeMenuButton.onClick.AddListener(OnCloseMenuClicked);
        closeMenuButton2.onClick.AddListener(OnCloseMenuClicked);

        destroyB = destroyButton.GetComponent<Button>();
        upgrade1 = upgradeButton1.GetComponent<Button>();
        upgrade2 = upgradeButton2.GetComponent<Button>();
        upgrade3 = upgradeButton3.GetComponent<Button>();
        pause = pauseButton.GetComponent<Button>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        
        cycleAnim.speed=0f;


    }

    void OnMouseDown()
    {
        if (UIdetection.instance.mouseOverUi){
            return;
        }
        if(buildState == 0){
        ShowBuildingMenu();
        } else {
            editing = true;
            ShowManageMenu();
        }
    }

    public void OnDestroyClicked(){
        if(editing == true){
            buildState = 0;
            buildID =0;
            level0 = 0;
            level1 = 1;
            level2 = 2;
            spriteRenderer.sprite = baseSprite;
            running = false;
            progress=0f;
            float timePassed=0f;

            HideManageMenu();
            
            editing = false;
        }
    }

    public void LevelUp1(Building building){
        if(editing == true){

                if (Materials.instance.mat_0 >= building.up_mat_0 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult)) && Materials.instance.mat_1 >= building.up_mat_1 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult)) && Materials.instance.mat_2 >= building.up_mat_2 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult)) && Materials.instance.price >= building.up_price * (1 + Mathf.RoundToInt(level0 * building.upgradeMult))){

        if (level0 != 3)
{
    if (level0 == 2)
    {
        if (mat_0_cycle < 0)
            mat_0_cycle += Mathf.RoundToInt(Mathf.Abs(mat_0_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp)));
        
        if (mat_1_cycle < 0)
            mat_1_cycle += Mathf.RoundToInt(Mathf.Abs(mat_1_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp)));
        
        if (mat_2_cycle < 0)
            mat_2_cycle += Mathf.RoundToInt(Mathf.Abs(mat_2_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp)));
        
        if (bar_0_cycle < 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));
        
        if (bar_1_cycle < 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));
        
        if (bar_2_cycle < 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));
        
        if (price_cycle < 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp)));

        level0 = 3;
    }
    else if (level0 == 1)
    {
        if (mat_0_cycle < 0)
            mat_0_cycle += Mathf.RoundToInt(Mathf.Abs(mat_0_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp)));
        
        if (mat_1_cycle < 0)
            mat_1_cycle += Mathf.RoundToInt(Mathf.Abs(mat_1_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp)));
        
        if (mat_2_cycle < 0)
            mat_2_cycle += Mathf.RoundToInt(Mathf.Abs(mat_2_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp)));
        
        if (bar_0_cycle < 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));
        
        if (bar_1_cycle < 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));
        
        if (bar_2_cycle < 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));
        
        if (price_cycle < 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp)));

        level0 = 2;
    }
    else if (level0 == 0)
    {
        if (mat_0_cycle < 0)
            mat_0_cycle += Mathf.RoundToInt(Mathf.Abs(mat_0_cycle * level1ScaleUp));
        
        if (mat_1_cycle < 0)
            mat_1_cycle += Mathf.RoundToInt(Mathf.Abs(mat_1_cycle * level1ScaleUp));
        
        if (mat_2_cycle < 0)
            mat_2_cycle += Mathf.RoundToInt(Mathf.Abs(mat_2_cycle * level1ScaleUp));
        
        if (bar_0_cycle < 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle * level1ScaleUp);
        
        if (bar_1_cycle < 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle * level1ScaleUp);
        
        if (bar_2_cycle < 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle * level1ScaleUp);
        
        if (price_cycle < 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle * level1ScaleUp));

        level0 = 1;
    }
    
        Materials.instance.mat_0 -= building.up_mat_0 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult));
        Materials.instance.mat_1 -= building.up_mat_1 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult));
        Materials.instance.mat_2 -= building.up_mat_2 * (1 + Mathf.RoundToInt(level0 * building.upgradeMult));
        Materials.instance.price -= building.up_price * (1 + Mathf.RoundToInt(level0 * building.upgradeMult));
}
                }
}

        
    }

    public void LevelUp2(Building building){
        if(editing == true){
                if (Materials.instance.mat_0 >= building.up_mat_0 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult)) && Materials.instance.mat_1 >= building.up_mat_1 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult)) && Materials.instance.mat_2 >= building.up_mat_2 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult)) && Materials.instance.price >= building.up_price * (1 + Mathf.RoundToInt(level1 * building.upgradeMult))){

        if (level1 != 3)
{
    if (level1 == 2)
    {
        if (bar_0_cycle > 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));

        if (bar_1_cycle > 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));

        if (bar_2_cycle > 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp));

        if (price_cycle > 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle / (1 + level2ScaleUp) * (level3ScaleUp - level2ScaleUp)));

        level1 = 3;
    }
    else if (level1 == 1)
    {
        if (bar_0_cycle > 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));

        if (bar_1_cycle > 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));

        if (bar_2_cycle > 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp));

        if (price_cycle > 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle / (1 + level1ScaleUp) * (level2ScaleUp - level1ScaleUp)));

        level1 = 2; 
    }
    else if (level1 == 0)
    {
        if (bar_0_cycle > 0)
            bar_0_cycle += Mathf.Abs(bar_0_cycle * level1ScaleUp);

        if (bar_1_cycle > 0)
            bar_1_cycle += Mathf.Abs(bar_1_cycle * level1ScaleUp);

        if (bar_2_cycle > 0)
            bar_2_cycle += Mathf.Abs(bar_2_cycle * level1ScaleUp);

        if (price_cycle > 0)
            price_cycle += Mathf.RoundToInt(Mathf.Abs(price_cycle * level1ScaleUp));

        level1 = 1; 
    }
    
        Materials.instance.mat_0 -= building.up_mat_0 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult));
        Materials.instance.mat_1 -= building.up_mat_1 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult));
        Materials.instance.mat_2 -= building.up_mat_2 * (1 + Mathf.RoundToInt(level1 * building.upgradeMult));
        Materials.instance.price -= building.up_price * (1 + Mathf.RoundToInt(level1 * building.upgradeMult)); 
}
                }
}
    }

    public void LevelUp3(Building building){
        if(editing == true){
                if (Materials.instance.mat_0 >= building.up_mat_0 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult)) && Materials.instance.mat_1 >= building.up_mat_1 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult)) && Materials.instance.mat_2 >= building.up_mat_2 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult)) && Materials.instance.price >= building.up_price * (1 + Mathf.RoundToInt(level2 * building.upgradeMult))){

        if (level2 != 3)
{
    if (level2 == 2)
    {
        cycleDuration = building.time * (1- level3ScaleUp);
        level2 = 3; 
    }
    if (level2 == 1)
    {
        cycleDuration = building.time * (1-level2ScaleUp);
        level2 = 2; 
    }
    if (level2 == 0)
    {
        cycleDuration = building.time * (1-level1ScaleUp);
        level2 = 1; 
    }

    
        Materials.instance.mat_0 -= building.up_mat_0 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult));
        Materials.instance.mat_1 -= building.up_mat_1 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult));
        Materials.instance.mat_2 -= building.up_mat_2 * (1 + Mathf.RoundToInt(level2 * building.upgradeMult));
        Materials.instance.price -= building.up_price * (1 + Mathf.RoundToInt(level2 * building.upgradeMult));

}
}}
    }


    private void ShowManageMenu()
    {
        if(editing == true){
        manageMenu.SetActive(true);
        foreach (Building building in buildings)
        {
            if (building.unlocked)  
            {
            destroyB.onClick.AddListener(() => OnDestroyClicked());
            
            int multiplier0 = Mathf.RoundToInt(level0 * building.upgradeMult);
            int multiplier1 = Mathf.RoundToInt(level1 * building.upgradeMult);
            int multiplier2 = Mathf.RoundToInt(level2 * building.upgradeMult);
            
            upgrade1.onClick.AddListener(() => LevelUp1(building));
        
        
            upgrade2.onClick.AddListener(() => LevelUp2(building));
        
           
            upgrade3.onClick.AddListener(() => LevelUp3(building));
            
            pause.onClick.AddListener(() => StopCycle());

            }
        
        }
        }
            
    }

    public void HideManageMenu()
    {
        manageMenu.SetActive(false);
    }





    private void ShowBuildingMenu()
    {
        buildingMenu.SetActive(true);
        closeMenu.SetActive(true); 

        foreach (Transform child in buttonPanel)
        {
            if (!child.CompareTag("ButtonPriority"))
    {
        Destroy(child.gameObject);
    }
        }

        foreach (Building building in buildings)
        {
            
            if (building.unlocked)  
            {
            GameObject newButton = Instantiate(buttonPrefab, buttonPanel);
            Button button = newButton.GetComponent<Button>();
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            buttonText.text = $"{building.name}: Wood {building.mat_0}, Stone {building.mat_1}, Iron {building.mat_2}, Price{building.price}";

            button.onClick.AddListener(() => OnBuildingButtonClick(building));
            }
        }
    }

    private void OnBuildingButtonClick(Building building)
    {
        if(Materials.instance.mat_0 >= (-1*building.mat_0) && Materials.instance.mat_1 >= (-1*building.mat_1) && Materials.instance.mat_2 >= (-1*building.mat_2) && Materials.instance.price >= (-1*building.price)){
        buildState = building.buildID;
        Color newColor;
        if (ColorUtility.TryParseHtmlString(building.debug, out newColor))
        {
            spriteRenderer.sprite = building.buildSprite;
        }
        Materials.instance.mat_0 += building.mat_0;
        Materials.instance.mat_1 += building.mat_1;
        Materials.instance.mat_2 += building.mat_2;
        Materials.instance.bar_0 += building.bar_0;
        Materials.instance.bar_1 += building.bar_1;
        Materials.instance.bar_2 += building.bar_2;
        Materials.instance.price += building.price;

        
        numbers[0] = building.mat_0;
        numbers[1] = building.mat_1;
        numbers[2] = building.mat_2;
        numbers[3] = building.price;
        StartCoroutine(SpawnPrefabs());

        mat_0_cycle = building.cons_mat_0;
        mat_1_cycle = building.cons_mat_1;
        mat_2_cycle = building.cons_mat_2;
        bar_0_cycle = building.bar_0_cycle;
        bar_1_cycle = building.bar_1_cycle;
        bar_2_cycle = building.bar_2_cycle;
        price_cycle = building.price_cycle;

        cycleDuration = building.time;


        running = true;

        StartCycle();

        HideBuildingMenu();
        }
        
        
    }


    private void OnCloseMenuClicked()
    {
        HideBuildingMenu();
        HideManageMenu();
        editing = false;
        
    }

    public void HideBuildingMenu()
    {
        foreach (Transform child in buttonPanel)
        {
            if (!child.CompareTag("ButtonPriority"))
    {
        Destroy(child.gameObject);
    }
        }
        buildingMenu.SetActive(false);
        closeMenu.SetActive(false);

    }



public void StartCycle()
    {
        running = true;
        cycleCoroutine = StartCoroutine(CycleRoutine());
    }

    IEnumerator CycleRoutine()
    {
        while (true)
        {
            float timePassed = 0;
            
            while ((timePassed < cycleDuration))
            {
                if (!running)
            {
                yield return null;
                continue; 
            }
                timePassed += Time.deltaTime;
                float progress = Mathf.Clamp01(timePassed / cycleDuration);
            
        cycleAnim.Play("CycleAnim",0,progress);

            if(Materials.instance.mat_0 < (-1*mat_0_cycle)){
                toFloat = true;
                running = false;
            }
            if(Materials.instance.mat_1 < (-1*mat_1_cycle)){
                toFloat = true;
                running = false;
            }
            if(Materials.instance.mat_2 < (-1*mat_2_cycle)){
                toFloat = true;
                running = false;
            }
            if(Materials.instance.price < (-1*price_cycle)){
                toFloat = true;
                running = false;
            }
                yield return null;
                if(buildState==0){
                    yield break;
                }

            }

            DoCycleAction();
        }
    }

    void DoCycleAction()
    {
        numbers[0] = mat_0_cycle;
        numbers[1] = mat_1_cycle;
        numbers[2] = mat_2_cycle;
        numbers[3] = price_cycle;
        Materials.instance.bar_0 += bar_0_cycle;
        Materials.instance.bar_1 += bar_1_cycle;
        Materials.instance.bar_2 += bar_2_cycle;
        Materials.instance.mat_0 += mat_0_cycle;
        Materials.instance.mat_1 += mat_1_cycle;
        Materials.instance.mat_2 += mat_2_cycle;
        Materials.instance.price += price_cycle;


        
        StartCoroutine(SpawnPrefabs());

    }

    public void SetCycleDuration(float newDuration)
    {
        cycleDuration = newDuration;
    }

    public void StopCycle()
    {
        running = !running;
    }






    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < 4; i++)
        {
            // Skip spawning if the number is 0
            if (numbers[i] == 0)
                continue;

            // Instantiate the prefab
            GameObject instance = Instantiate(prefabToSpawn, transform.position + spawnPosition + new Vector3(positionOffset, 0f, 0f) * i, Quaternion.identity);

            // Set the text and sprite for the instance
            TextMesh textMesh = instance.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                if (numbers[i] < 0){
                textMesh.color = Color.red;
                } else {
                textMesh.color = Color.green;
                }
                textMesh.text = (numbers[i] >= 0 ? "+" : "") + numbers[i].ToString();

            }

            SpriteRenderer spriteRenderer = instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[i];
            }

            // Start fading out and moving up
            StartCoroutine(FadeOutAndMove(instance, numbers[i]));

            // Wait for the time offset before spawning the next one
            yield return new WaitForSeconds(timeOffset);
        }
    }

    private IEnumerator FadeOutAndMove(GameObject instance,  float numberVal)
    {
        float elapsedTime = 0f;
        TextMesh textMesh = instance.GetComponent<TextMesh>();
        SpriteRenderer spriteRenderer = instance.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Color textColor = textMesh.color;
        Color spriteColor = spriteRenderer.color;

        Vector3 initialPosition = instance.transform.position;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            // Set the transparency
            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            // Move the instance up
            if (numberVal > 0){
            instance.transform.position = initialPosition + Vector3.up * moveSpeed * (elapsedTime / fadeDuration);
            } else {
            instance.transform.position = initialPosition + Vector3.down * moveSpeed * (elapsedTime / fadeDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's completely invisible and destroy the instance
        textMesh.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0f);
        Destroy(instance);
    }
}
