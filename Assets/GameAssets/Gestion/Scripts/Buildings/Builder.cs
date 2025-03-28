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
    [SerializeField] public Sprite baseSprite;

    private SpriteRenderer spriteRenderer;
    public AudioClip sfxClip;
    public AudioClip destroy;
    public AudioClip clic;
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

    public int buildClass;

    private float level1ScaleUp = 0.2f;
    private float level2ScaleUp = 0.4f;
    private float level3ScaleUp = 0.6f;


    public bool toFloat = false;

    public Animator cycleAnim;

    public float progress;

    private bool isMenuOpen = false;

    public float outsideTime = 0;

    public bool editing = false;




    public GameObject prefabToSpawn;
    public Vector3 spawnPosition;
    public float positionOffset = 40f;
    public float timeOffset = 0.2f;
    public float fadeDuration = 1f;
    public float moveSpeed = 1f;


    private float[] numbers = new float[4];
    public Sprite[] sprites = new Sprite[4];
    public TMP_Text buildingEtatMoney;
    public TMP_Text buildingNameText;
    public TMP_Text buildingDescriptionText;
    public TMP_Text buildingEtatPolution;
    public TMP_Text buildingEtatPopulation;
    public TMP_Text buildingEtatElec;
    public int Prebuild = 0;


    public GameObject cycleBar;

    public GameObject menuRecherche;
    public GameObject PauseInfo;
    public GameObject notEnothRessourse;
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private GameObject bar0Prefab;
    [SerializeField] private GameObject bar1Prefab;
    [SerializeField] private bool tutorialBuild;
    public TMP_Text mat0Text;
    public TMP_Text mat1Text;
    public TMP_Text mat2Text;
    public TMP_Text priceText;
    public TMP_Text timeText;

    public Sprite normalDestroySprite;
    public Sprite lockedDestroySprite;

    public SpriteRenderer spriteRendererDelete;
    public GameObject NoBuildingText;

    public Button closeButton;
    public SpriteRenderer barFs;
    public SpriteRenderer barBs;
    public SpriteRenderer Barfond;
    public SpriteRenderer sizeIcon1;
    public SpriteRenderer sizeIcon2;

    public Sprite big;
    public Sprite water;
    public Sprite norm;

    private void Awake()
    {

        barFs = cycleBar.transform.Find("CycleBarP").GetComponent<SpriteRenderer>();
        barBs = cycleBar.transform.Find("CycleBarP (1)").GetComponent<SpriteRenderer>();
        Barfond = cycleBar.transform.Find("Static Sprite").GetComponent<SpriteRenderer>();
        closeMenuButton = closeMenu.GetComponent<Button>();
        closeMenuButton2 = closeMenu2.GetComponent<Button>();

        closeMenuButton.onClick.RemoveAllListeners();
        closeMenuButton.onClick.AddListener(OnCloseMenuClicked);

        closeMenuButton2.onClick.RemoveAllListeners();
        closeMenuButton2.onClick.AddListener(OnCloseMenuClicked);
        destroyB = destroyButton.GetComponent<Button>();
        upgrade1 = upgradeButton1.GetComponent<Button>();
        upgrade2 = upgradeButton2.GetComponent<Button>();
        upgrade3 = upgradeButton3.GetComponent<Button>();
        pause = pauseButton.GetComponent<Button>();

        spriteRenderer = GetComponent<SpriteRenderer>();


        cycleAnim.speed = 0f;
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                HideBuildingMenu();
            });
        }
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
        if (buildState == 50)
        {
            Materials.instance.researchCentr = false;
            Materials.instance.ReseachButton(true);
        }
    }

    void Update()
    {
        isMenuOpen = manageMenu.activeSelf;

        if (editing)
        {
            Building buildingToDestroy = GetBuildingByID(buildState);

            if (buildingToDestroy != null)
            {
                float destroyCost = buildingToDestroy.price * -10f;

                if (Materials.instance.price < (int)destroyCost)
                {
                    UpdateDestroyButtonSprite(false);
                }
                else
                {
                    UpdateDestroyButtonSprite(true);
                }
            }
        }

        if (buildState == 50)
        {
            Materials.instance.researchCentr = false;
            Materials.instance.ReseachButton(true);
        }
        foreach (Building building in buildings)
        {
            PauseInfo.transform.localPosition = new Vector3(10, 100, 0);
            notEnothRessourse.transform.localPosition = new Vector3(40, 100, 0);

            if (buildState != 0)
            {
                if (running == false)
                {
                    PauseInfo.SetActive(true);
                    pauseImage.sprite = playSprite;
                }
                else
                {
                    pauseImage.sprite = pauseSprite;
                    PauseInfo.SetActive(false);
                }
                CheckAndDisplayMissingResources();
            }
        }
    }
    private bool wasMissingResources = false;
    void CheckAndDisplayMissingResources()
    {
        if (notEnothRessourse == null)
        {
            Debug.LogError("notEnothRessourse n'est pas assigné dans l'Inspector !");
            return;
        }

        bool isMissing = Materials.instance.price < (-1 * price_cycle) ||
                         Materials.instance.bar_0 < (-1 * bar_0_cycle) ||
                         Materials.instance.bar_1 < (-1 * bar_1_cycle);

        if (isMissing == wasMissingResources)
            return;

        wasMissingResources = isMissing;

        foreach (Transform child in notEnothRessourse.transform)
        {
            Destroy(child.gameObject);
        }

        if (Materials.instance.price < (-1 * price_cycle))
        {
            GameObject obj = Instantiate(moneyPrefab, notEnothRessourse.transform);
            obj.transform.localPosition = Vector3.zero;
        }

        if (Materials.instance.bar_0 < (-1 * bar_0_cycle))
        {
            GameObject obj = Instantiate(bar0Prefab, notEnothRessourse.transform);
            obj.transform.localPosition = Vector3.zero;
        }

        if (Materials.instance.bar_1 < (-1 * bar_1_cycle))
        {
            GameObject obj = Instantiate(bar1Prefab, notEnothRessourse.transform);
            obj.transform.localPosition = Vector3.zero;
        }

        if (isMissing)
        {
            notEnothRessourse.SetActive(true);
            notEnothRessourse.transform.localPosition = Vector3.zero;
        }
        else
        {
            notEnothRessourse.SetActive(false);
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
                editing = true;
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
        SoundManager.instance.PlaySFX(clic);
        MoreInfoClose.onClick.AddListener(() => OnMoreInfoCloseClicked());
        moreInfoPanel.SetActive(true);
    }
    public void OnMoreInfoCloseClicked()
    {
        SoundManager.instance.PlaySFX(clic);
        moreInfoPanel.SetActive(false);
    }


    public void OnDestroyClicked()
    {
        if (editing)
        {
            Building buildingToDestroy = GetBuildingByID(buildState);

            if (buildingToDestroy != null)
            {
                float destroyCost = buildingToDestroy.price * -10f;

                if (Materials.instance.price < (int)destroyCost)
                {
                    UpdateDestroyButtonSprite(false);
                    return;
                }

                Materials.instance.price -= (int)destroyCost;
                SoundManager.instance.PlaySFX(destroy);
                Materials.instance.mat_0 -= (int)(buildingToDestroy.mat_0 / 2f);
                Materials.instance.mat_1 -= (int)(buildingToDestroy.mat_1 / 2f);
                Materials.instance.mat_2 -= (int)(buildingToDestroy.mat_2 / 2f);


                if (buildState == 50)
                {
                    Materials.instance.researchCentr = true;
                    Materials.instance.ReseachButton(false);
                }

                cycleBar.transform.localPosition = Vector3.zero;
                barFs.sortingOrder = -10;
                barBs.sortingOrder = -11;
                Barfond.sortingOrder = -12;
                buildState = 0;
                buildID = 0;
                level0 = 0;
                level1 = 1;
                level2 = 2;

                if (!Materials.instance.explored)
                {
                    spriteRenderer.sprite = baseSprite;
                }
                PauseInfo.SetActive(false);
                notEnothRessourse.SetActive(false);
                running = false;
                progress = 0f;

                HideManageMenu();
                editing = false;
                Materials.instance.canMove = true;
            }
        }
    }


    private void UpdateDestroyButtonSprite(bool canDestroy)
    {
        if (canDestroy)
        {
            spriteRendererDelete.sprite = normalDestroySprite;
        }
        else
        {
            spriteRendererDelete.sprite = lockedDestroySprite;
        }
    }

    private Building GetBuildingByID(int id)
    {
        foreach (Building building in buildings)
        {
            if (building.buildID == id)
                return building;
        }
        return null;
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
        Debug.Log(isMenuOpen);
        if (editing && !isMenuOpen)
        {
            isMenuOpen = true; // Empêche d’ouvrir le menu plusieurs fois

            SoundManager.instance.PlaySFX(clic);
            if (buildState == 50)
            {
                menuRecherche.SetActive(true);
                return;
            }

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

                    if (!running)
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

                    Building buildingToDestroy = GetBuildingByID(buildState);

                    if (mat0Text != null)
                        mat0Text.text = (buildingToDestroy.mat_0 / 2f * -1).ToString("F0");

                    if (mat1Text != null)
                        mat1Text.text = (buildingToDestroy.mat_1 / 2f * -1).ToString("F0");

                    if (mat2Text != null)
                        mat2Text.text = (buildingToDestroy.mat_2 / 2f * -1).ToString("F0");

                    if (priceText != null)
                        priceText.text = (building.price * 10f).ToString("F0");

                    if (timeText != null)
                    {
                        timeText.text = building.time + "s";
                    }

                    StartCoroutine(MoveCameraToBuilding(transform.position));
                }
            }
        }
    }




    private IEnumerator MoveCameraToBuilding(Vector3 targetPosition)
    {
        Vector3 startPosition = mainCamera.transform.position;
        Vector3 targetPos = new Vector3(targetPosition.x + 200f, targetPosition.y, mainCamera.transform.position.z);

        float elapsedTime = 0;
        float duration = 1.5f;

        while (elapsedTime < duration)
        {
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime * cameraMoveSpeed;
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
        isMenuOpen = false;
        manageMenu.SetActive(false);
    }


    private void ShowBuildingMenu()
    {
        if (buildings[0].buildID == 1)
        {
            sizeIcon1.sprite = big;
            sizeIcon2.sprite = big;
        }
        else if (buildings[0].buildID == 3)
        {
            sizeIcon1.sprite = water;
            sizeIcon2.sprite = water;
        }
        else
        {
            sizeIcon1.sprite = norm;
            sizeIcon2.sprite = norm;

        }
        Debug.Log(buildings[0].buildID);
        SoundManager.instance.PlaySFX(clic);
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
                    continue;
                }
                else if (!Materials.instance.researchCentr && building.buildClass == 0)
                {
                    continue;
                }

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
                    if (child.CompareTag("ImpactDisplay"))
                    {
                        TextMeshProUGUI tmp = child.GetComponentInChildren<TextMeshProUGUI>();

                        switch (child.name)
                        {
                            case "airImpact":
                                if (building.bar_2_cycle < 0)
                                {
                                    tmp.text = "-";
                                    tmp.color = Color.blue;
                                }
                                break;
                            case "energyImpact":
                                if (building.bar_1_cycle < 0)
                                {
                                    tmp.text = "-";
                                    tmp.color = Color.red;
                                }
                                break;
                            case "qolImpact":
                                if (building.bar_0_cycle < 0)
                                {
                                    tmp.text = "-";
                                    tmp.color = Color.red;
                                }
                                break;
                            case "moneyImpact":
                                if (building.price_cycle < 0)
                                {
                                    tmp.text = "-";
                                    tmp.color = Color.red;
                                }
                                break;
                        }
                    }
                }

                if (building.buildID == 50)
                {
                    Transform explanationText = newButton.transform.Find("ExpliationText");
                    if (explanationText != null)
                    {
                        explanationText.gameObject.SetActive(true);
                    }

                    string[] impactNames = { "energyImpact", "airImpact", "moneyImpact", "qolImpact", "Impacts (TMP)" };
                    foreach (string impactName in impactNames)
                    {
                        Transform impactTransform = newButton.transform.Find(impactName);
                        if (impactTransform != null)
                        {
                            impactTransform.gameObject.SetActive(false);
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

        Transform cardSlider = buildingMenu.transform.Find("CardSlider");
        if (cardSlider != null)
        {
            Transform movingScroll = cardSlider.Find("MovingScroll");
            if (movingScroll != null)
            {
                bool foundButton = false;

                foreach (Transform child in movingScroll)
                {
                    if (child.name == "Button(Clone)")
                    {
                        foundButton = true;
                        break;
                    }
                }

                if (!foundButton)
                {
                    NoBuildingText.SetActive(true);
                }
                else
                {
                    NoBuildingText.SetActive(false);
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
        if (buttonObject != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonObject);

            validationButton.onClick.RemoveAllListeners();

            validationButton.onClick.AddListener(() => OnBuildingButtonClick(building));
        }

    }


    private void OnBuildingButtonClick(Building building)
    {
        if (building.buildClass == 0)
        {
            Materials.instance.researchCentr = false;
            Materials.instance.ReseachButton(true);
        }
        if (building.buildID != 50)
        {
            cycleBar.transform.localPosition = new Vector3(0, 83, 0);
            barFs.sortingOrder = -2;
            barBs.sortingOrder = -3;
            Barfond.sortingOrder = -4;
        }

        if (editing == true && (Materials.instance.mat_0 >= (-1 * building.mat_0) &&
             Materials.instance.mat_1 >= (-1 * building.mat_1) &&
             Materials.instance.mat_2 >= (-1 * building.mat_2) &&
             Materials.instance.price >= (-1 * building.price)) || Prebuild != 0)
        {
            buildState = building.buildID;
            buildClass = building.buildClass;
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
            }

            HideBuildingMenu();
            editing = false;
            Materials.instance.canMove = true;
        }
    }



    private void OnCloseMenuClicked()
    {
        SoundManager.instance.PlaySFX(clic);
        HideBuildingMenu();
        HideManageMenu();
        editing = false;
        isMenuOpen = false;
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

                if (Materials.instance.mat_0 < (-1 * mat_0_cycle) || Materials.instance.mat_1 < (-1 * mat_1_cycle) || Materials.instance.mat_2 < (-1 * mat_2_cycle) || Materials.instance.price < (-1 * price_cycle) || Materials.instance.bar_0 < (-1 * bar_0_cycle) || Materials.instance.bar_1 < (-1 * bar_1_cycle))
                {
                    running = false;
                }
                yield return null;
                if (buildState == 0)
                {
                    yield break;
                }
                if (toFloat){
                    running = false;
                    toFloat = false;
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
        SoundManager.instance.PlaySFX(clic);
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
        SoundManager.instance.PlaySFX(clic);
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
