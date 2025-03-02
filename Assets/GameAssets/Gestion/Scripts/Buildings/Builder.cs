using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour
{
    public Camera mainCamera;
    public float cameraMoveSpeed = 2.0f;
    public GameObject moreInfoPanel;
    [SerializeField] private TextMeshProUGUI price0;
    [SerializeField] private TextMeshProUGUI price1;
    [SerializeField] private TextMeshProUGUI price2;
    [SerializeField] private TextMeshProUGUI price3;
    [SerializeField] private GameObject validation;
    [SerializeField] private Button validationButton;
    [SerializeField] private GameObject validationButtonObject;
    [SerializeField] private GameObject validationPriceNo;
    [SerializeField]
    private GameObject unvalidText
    ;

    [SerializeField] private Transform buttonCont;

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
    public AudioClip sfxClip;
    public int buildState = 0;
    private int buildID = 0;

    private Button closeMenuButton;
    private Button closeMenuButton2;
    private Button destroyB;
    public Button MoreInfo;
    public Button MoreInfoClose;
    private Button upgrade1;
    private Button upgrade2;
    private Button upgrade3;
    private Button pause;

    public Image pauseImage;
    public Sprite playSprite;
    public Sprite pauseSprite;

    public bool running = false;
    public float cycleDuration = 1f;
    private Coroutine cycleCoroutine;
    public int mat_0_cycle = 0;
    public int mat_1_cycle = 0;
    public int mat_2_cycle = 0;
    public float bar_0_cycle = 0f;
    public float bar_1_cycle = 0f;
    public float bar_2_cycle = 0f;
    public int price_cycle = 0;
    public int level0 = 0;
    public int level1 = 0;
    public int level2 = 0;

    private float level1ScaleUp = 0.2f;
    private float level2ScaleUp = 0.4f;
    private float level3ScaleUp = 0.6f;


    private bool toFloat = false;

    public Animator cycleAnim;

    public float progress;


    public float outsideTime = 0;

    public bool editing = false;




    public GameObject prefabToSpawn;           // The prefab to spawn
    public Vector3 spawnPosition;              // The base position to spawn prefabs at
    public float positionOffset = 40f;          // The offset position for each new spawn
    public float timeOffset = 0.2f;            // Delay between each spawn
    public float fadeDuration = 1f;            // Duration for fading out
    public float moveSpeed = 1f;               // Speed at which the object moves upward


    private float[] numbers = new float[4];    // Array to store numbers to display
    public Sprite[] sprites = new Sprite[4];
    public TMP_Text buildingEtatMoney;
    public TMP_Text buildingNameText;
    public TMP_Text buildingDescriptionText;
    public TMP_Text buildingEtatPolution;
    public TMP_Text buildingEtatPopulation;
    public TMP_Text buildingEtatElec;
    public int Prebuild = 0;


    public GameObject cycleBar;
    [SerializeField] private bool tutorialBuild;



    private void Awake()
    {
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


        cycleAnim.speed = 0f;

        if (Materials.instance.isLoad)
        {
            Prebuild = 0;
        }
        if (Prebuild != 0)
        {
            Building matchingBuilding = null;

            foreach (Building building in buildings)
            {
                if (building.buildID == Prebuild)
                {
                    matchingBuilding = building;
                    break;
                }
            }
            OnBuildingButtonClick(matchingBuilding);
        }


    }

    void OnMouseDown()
    {
        if ((!Materials.instance.tutorial && Materials.instance.textDone) || (tutorialBuild && Materials.instance.tutoToggle))
        {
            if (UIdetection.instance.mouseOverUi)
            {
                return;
            }
            if (buildState == 0)
            {
                ShowBuildingMenu();
            }
            else
            {
                editing = true;
                ShowManageMenu();
            }
        }
    }

    public void OnMoreInfoClicked()
    {
        MoreInfoClose.onClick.AddListener(() => OnMoreInfoCloseClicked());
        moreInfoPanel.SetActive(true);
    }
    public void OnMoreInfoCloseClicked()
    {
        moreInfoPanel.SetActive(false);
    }


    public void OnDestroyClicked()
    {
        if (editing == true)
        {
            cycleBar.transform.localPosition = new Vector3(0, -46, 0);
            buildState = 0;
            buildID = 0;
            level0 = 0;
            level1 = 1;
            level2 = 2;
            if (Materials.instance.explored == false)
            {
                spriteRenderer.sprite = baseSprite;
            }
            running = false;
            progress = 0f;
            float timePassed = 0f;

            HideManageMenu();

            editing = false;
            Materials.instance.canMove = true;
        }
    }





    private bool resourceCheckUpgrade(int level, Building building)
    { // Check si le joueur a assez de matériaux pour améliorer son bâtiment
        int priceMult = (1 + Mathf.RoundToInt(level0 * building.upgradeMult));
        bool Mat0Up = Materials.instance.mat_0 >= building.up_mat_0 * priceMult;
        bool Mat1Up = Materials.instance.mat_1 >= building.up_mat_1 * priceMult;
        bool Mat2Up = Materials.instance.mat_2 >= building.up_mat_2 * priceMult;
        bool PriceUp = Materials.instance.price >= building.up_price * priceMult;

        return Mat0Up && Mat1Up && Mat2Up && PriceUp;
    }

    private int UpMat(int mat, float levelScaleUp1, float levelScaleUp2)
    {
        if (mat < 0)
        {
            return mat + Mathf.RoundToInt(Mathf.Abs(mat / (1 + levelScaleUp1) * (levelScaleUp2 - levelScaleUp1)));
        }
        return mat;
    }

    private float UpBar(float bar, float levelScaleUp1, float levelScaleUp2)
    {
        if (bar < 0)
        {
            return bar + Mathf.Abs(bar / (1 + levelScaleUp1) * (levelScaleUp2 - levelScaleUp1));
        }
        return bar;
    }



    private void ChargeUp(int level, Building building)
    {//enlève les matériaux

        Materials.instance.mat_0 -= building.up_mat_0 * (1 + Mathf.RoundToInt(level * building.upgradeMult));
        Materials.instance.mat_1 -= building.up_mat_1 * (1 + Mathf.RoundToInt(level * building.upgradeMult));
        Materials.instance.mat_2 -= building.up_mat_2 * (1 + Mathf.RoundToInt(level * building.upgradeMult));
        Materials.instance.price -= building.up_price * (1 + Mathf.RoundToInt(level * building.upgradeMult));
    }

    public void LevelUp1(Building building)
    {
        if (editing == true)
        {

            if (resourceCheckUpgrade(level0, building))
            {

                if (level0 != 3)
                {
                    float levelScaleUp1 = 0;
                    float levelScaleUp2 = level1ScaleUp;

                    if (level0 == 1)
                    {
                        levelScaleUp1 = level1ScaleUp;
                        levelScaleUp2 = level2ScaleUp;
                    }
                    else if (level0 == 2)
                    {
                        levelScaleUp1 = level2ScaleUp;
                        levelScaleUp2 = level3ScaleUp;
                    }

                    mat_0_cycle = UpMat(mat_0_cycle, levelScaleUp1, levelScaleUp2);
                    mat_1_cycle = UpMat(mat_1_cycle, levelScaleUp1, levelScaleUp2);
                    mat_2_cycle = UpMat(mat_2_cycle, levelScaleUp1, levelScaleUp2);
                    bar_0_cycle = UpBar(bar_0_cycle, levelScaleUp1, levelScaleUp2);
                    bar_1_cycle = UpBar(bar_1_cycle, levelScaleUp1, levelScaleUp2);
                    bar_2_cycle = UpBar(bar_2_cycle, levelScaleUp1, levelScaleUp2);
                    price_cycle = UpMat(price_cycle, levelScaleUp1, levelScaleUp2);
                    level0++;
                    ChargeUp(level0, building);
                }
            }
        }


    }

    public void LevelUp2(Building building)
    {
        if (editing == true)
        {
            if (resourceCheckUpgrade(level1, building))
            {

                if (level1 != 3)
                {
                    float levelScaleUp1 = 0;
                    float levelScaleUp2 = level1ScaleUp;

                    if (level1 == 1)
                    {
                        levelScaleUp1 = level1ScaleUp;
                        levelScaleUp2 = level2ScaleUp;
                    }
                    else if (level1 == 2)
                    {
                        levelScaleUp1 = level2ScaleUp;
                        levelScaleUp2 = level3ScaleUp;
                    }

                    bar_0_cycle = UpBar(bar_0_cycle, levelScaleUp1, levelScaleUp2);
                    bar_1_cycle = UpBar(bar_1_cycle, levelScaleUp1, levelScaleUp2);
                    bar_2_cycle = UpBar(bar_2_cycle, levelScaleUp1, levelScaleUp2);

                    level1++;
                    ChargeUp(level1, building);
                }
            }
        }
    }

    private void UpTime(float timeScaleUp, Building building)
    {
        cycleDuration = building.time * (1 - timeScaleUp);
        level2++;
        ChargeUp(level2, building);
    }

    public void LevelUp3(Building building)
    {
        if (editing == true)
        {
            if (resourceCheckUpgrade(level2, building))
            {

                if (level2 != 3)
                {
                    if (level2 == 2)
                    {
                        UpTime(level3ScaleUp, building);
                    }
                    if (level2 == 1)
                    {
                        UpTime(level2ScaleUp, building);
                    }
                    if (level2 == 0)
                    {
                        UpTime(level1ScaleUp, building);
                    }



                    ChargeUp(level2, building);

                }
            }
        }
    }



    private void ShowManageMenu()
    {
        if (editing)
        {
            Materials.instance.canMove = false;
            manageMenu.SetActive(true);

            foreach (Building building in buildings)
            {
                if (building.unlocked && building.buildID == buildState)
                {
                    destroyB.onClick.RemoveAllListeners();
                    MoreInfo.onClick.RemoveAllListeners();
                    upgrade1.onClick.RemoveAllListeners();
                    upgrade2.onClick.RemoveAllListeners();
                    upgrade3.onClick.RemoveAllListeners();
                    pause.onClick.RemoveAllListeners();

                    destroyB.onClick.AddListener(() => OnDestroyClicked());
                    MoreInfo.onClick.AddListener(() => OnMoreInfoClicked());

                    upgrade1.onClick.AddListener(() => LevelUp1(building));
                    upgrade2.onClick.AddListener(() => LevelUp2(building));
                    upgrade3.onClick.AddListener(() => LevelUp3(building));

                    if (building.isPaused)
                    {
                        pauseImage.sprite = playSprite;
                        pause.onClick.AddListener(() => ContinueCycle());
                    }
                    else
                    {
                        pauseImage.sprite = pauseSprite;
                        pause.onClick.AddListener(() => StopCycle());
                    }

                    buildingNameText.text = building.name;
                    buildingDescriptionText.text = building.buildDesc;

                    UpdateTextColor(buildingEtatPolution, building.PolutionEtat, true);
                    UpdateTextColor(buildingEtatPopulation, building.PopulationEtat, false);
                    UpdateTextColor(buildingEtatElec, building.ElecEtat, false);
                    UpdateTextColor(buildingEtatMoney, building.MoneyMake, false);

                    StartCoroutine(MoveCameraToBuilding(gameObject.transform.position));
                }
            }
        }
    }

    private IEnumerator MoveCameraToBuilding(Vector3 targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x + 200f, targetPosition.y + 0f, mainCamera.transform.position.z);

        float elapsedTime = 0;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime * cameraMoveSpeed;
            yield return null;
        }

        mainCamera.transform.position = targetPos;
    }

    private void UpdateTextColor(TMP_Text textElement, string value, bool invertColors)
    {
        textElement.text = value;

        if (invertColors)
        {
            if (value.Contains("-"))
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.blue;
            }
            else if (value.Contains("+"))
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.red;
            }
            else
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.black;
            }
        }
        else
        {
            if (value.Contains("-"))
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.red;
            }
            else if (value.Contains("+"))
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.blue;
            }
            else
            {
                textElement.outlineWidth = 0.3f;
                textElement.outlineColor = Color.white;
                textElement.color = Color.black;
            }
        }
    }


    public void HideManageMenu()
    {
        manageMenu.SetActive(false);
    }





    private void ShowBuildingMenu()
    {
        validation.SetActive(false);
        buildingMenu.SetActive(true);
        closeMenu.SetActive(true);

        Materials.instance.canMove = false;

        foreach (Transform child in buttonPanel)
        {
            if (!child.CompareTag("ButtonPriority"))
            {
                Destroy(child.gameObject);
            }
        }
        int counter = 0;

        foreach (Building building in buildings)
        {


            if (building.unlocked)
            {
                if (Materials.instance.researchCentr && building.buildClass != 0)
                {

                }
                else if (!Materials.instance.researchCentr && building.buildClass == 0) { }
                else
                {
                    GameObject newButton = Instantiate(buttonPrefab, buttonCont);
                    newButton.transform.localPosition += new Vector3(-280f + (counter * 280f), 69f, 0f);
                    Button button = newButton.GetComponent<Button>();
                    TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
                    Transform buttonSprite = newButton.transform.Find("BuildIMG");
                    Image buttonImage = buttonSprite.GetComponent<Image>();
                    buttonImage.sprite = building.buildSprite;
                    buttonText.text = $"{building.name}";
                    counter += 1;

                    foreach (Transform child in newButton.transform)
                    {
                        if (child.CompareTag("ImpactDisplay") && child.name == "airImpact")
                        {
                            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                            if (building.bar_2_cycle < 0)
                            {
                                tmp.text = "-";
                                tmp.color = Color.blue;
                            }
                        }
                        if (child.CompareTag("ImpactDisplay") && child.name == "energyImpact")
                        {
                            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                            if (building.bar_1_cycle < 0)
                            {
                                tmp.text = "-";
                                tmp.color = Color.red;
                            }
                        }
                        if (child.CompareTag("ImpactDisplay") && child.name == "qolImpact")
                        {
                            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                            if (building.bar_0_cycle < 0)
                            {
                                tmp.text = "-";
                                tmp.color = Color.red;
                            }
                        }
                        if (child.CompareTag("ImpactDisplay") && child.name == "moneyImpact")
                        {
                            TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                            if (building.price_cycle < 0)
                            {
                                tmp.text = "-";
                                tmp.color = Color.red;
                            }
                        }
                    }


                    if (Materials.instance.tutorial)
                    {
                        button.interactable = false;
                    }

                    button.onClick.AddListener(() => SelectBuild(building, button.gameObject));
                }
            }

        }
    }

    public void SelectBuild(Building building, GameObject buttonObject)
    {
        EventSystem.current.SetSelectedGameObject(null);
        validation.SetActive(true);
        validationPriceNo.SetActive(false);
        if ((Materials.instance.mat_0 >= (-1 * building.mat_0) && Materials.instance.mat_1 >= (-1 * building.mat_1) && Materials.instance.mat_2 >= (-1 * building.mat_2) && Materials.instance.price >= (-1 * building.price)))
        {
            validationButtonObject.SetActive(true);
            unvalidText.SetActive(false);

        }
        else
        {

            validationButtonObject.SetActive(false);
            unvalidText.SetActive(true);
        }
        price0.text = string.Empty;
        price1.text = string.Empty;
        price2.text = string.Empty;
        price3.text = string.Empty;
        price0.text = building.mat_0.ToString();
        price1.text = building.mat_1.ToString();
        price2.text = building.mat_2.ToString();
        price3.text = building.price.ToString();
        EventSystem.current.SetSelectedGameObject(buttonObject);
        validationButton.onClick.AddListener(() => OnBuildingButtonClick(building));

    }


    private void OnBuildingButtonClick(Building building)
    {
        if (building.buildClass == 0)
        {
            Materials.instance.researchCentr = false;
        }

        cycleBar.transform.localPosition = new Vector3(0, 83, 0);

        if ((Materials.instance.mat_0 >= (-1 * building.mat_0) &&
             Materials.instance.mat_1 >= (-1 * building.mat_1) &&
             Materials.instance.mat_2 >= (-1 * building.mat_2) &&
             Materials.instance.price >= (-1 * building.price)) || Prebuild != 0)
        {
            buildState = building.buildID;
            spriteRenderer.sprite = building.buildSprite;

            if (Prebuild == 0)
            {
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

                SoundManager.instance.PlaySFX(sfxClip);
            }
            else
            {
                Prebuild = 0;
            }

            mat_0_cycle = building.cons_mat_0;
            mat_1_cycle = building.cons_mat_1;
            mat_2_cycle = building.cons_mat_2;
            bar_0_cycle = building.bar_0_cycle;
            bar_1_cycle = building.bar_1_cycle;
            bar_2_cycle = building.bar_2_cycle;
            price_cycle = building.price_cycle;
            cycleDuration = building.time;
            running = !building.isPaused;
            if (running)
            {
                StartCycle();
                Debug.Log(building.buildID);
            }

            HideBuildingMenu();
            Materials.instance.canMove = true;
        }
    }



    private void OnCloseMenuClicked()
    {
        HideBuildingMenu();
        HideManageMenu();
        editing = false;

        Materials.instance.canMove = true;

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


    public void Testconn()
    {
        StartCycle();
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

                cycleAnim.Play("CycleAnim", 0, progress);

                if (Materials.instance.mat_0 < (-1 * mat_0_cycle) || Materials.instance.mat_1 < (-1 * mat_1_cycle) || Materials.instance.mat_2 < (-1 * mat_2_cycle) || Materials.instance.price < (-1 * price_cycle))
                {
                    toFloat = true;
                    running = false;
                }
                yield return null;
                if (buildState == 0)
                {
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
        numbers[2] = price_cycle;
        Materials.instance.bar_0 += bar_0_cycle;
        Materials.instance.bar_1 += bar_1_cycle;
        Materials.instance.bar_2 += bar_2_cycle;
        Materials.instance.mat_0 += mat_0_cycle;
        Materials.instance.mat_1 += mat_1_cycle;
        Materials.instance.mat_2 += mat_2_cycle;
        Materials.instance.price += price_cycle;


        StartCoroutine(SpawnPrefabs());

    }

    public void StopCycle()
    {
        running = false;
        if (buildState != 0)
        {
            Building currentBuilding = buildings.Find(b => b.buildID == buildState);
            if (currentBuilding != null)
            {
                currentBuilding.isPaused = true;
                pauseImage.sprite = playSprite;
            }
        }

        pause.onClick.RemoveAllListeners();
        pause.onClick.AddListener(() => ContinueCycle());
    }

    public void ContinueCycle()
    {

        running = true;
        if (buildState != 0)
        {

            Building currentBuilding = buildings.Find(b => b.buildID == buildState);
            if (currentBuilding != null)
            {
                currentBuilding.isPaused = false;
                pauseImage.sprite = pauseSprite;
            }
        }

        pause.onClick.RemoveAllListeners();
        pause.onClick.AddListener(() => StopCycle());
    }




    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < 4; i++)
        {
            if (numbers[i] == 0)
                continue;
            GameObject instance = Instantiate(prefabToSpawn, transform.position + spawnPosition + new Vector3(positionOffset, 0f, 0f) * i, Quaternion.identity);

            TextMesh textMesh = instance.GetComponent<TextMesh>();
            if (textMesh != null)
            {
                if (numbers[i] < 0)
                {
                    textMesh.color = Color.red;
                }
                else
                {
                    textMesh.color = Color.green;
                }
                textMesh.text = (numbers[i] >= 0 ? "+" : "") + numbers[i].ToString();

            }

            SpriteRenderer spriteRenderer = instance.transform.GetChild(0).GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = sprites[i];
            }

            StartCoroutine(FadeOutAndMove(instance, numbers[i]));

            yield return new WaitForSecondsRealtime(timeOffset);
        }
    }

    private IEnumerator FadeOutAndMove(GameObject instance, float numberVal)
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

            textMesh.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            if (numberVal > 0)
            {
                instance.transform.position = initialPosition + Vector3.up * moveSpeed * (elapsedTime / fadeDuration);
            }
            else
            {
                instance.transform.position = initialPosition + Vector3.down * moveSpeed * (elapsedTime / fadeDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        textMesh.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        spriteRenderer.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 0f);
        Destroy(instance);
    }
}
