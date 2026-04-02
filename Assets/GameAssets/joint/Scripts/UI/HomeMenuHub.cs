using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class HomeMenuHub : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip sfxClip;

    [Header("Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button returnToMenuButton;

    [Header("Visual Settings")]
    public Color normalTextColor = Color.black;
    public Color hoverTextColor = Color.blue;
    public Color specialHoverTextColor = Color.red;
    public Color outlineColor = Color.white;

    [Header("Menu & Navigation")]
    public GameObject canvas;
    public GameObject canvasCredit;
    public GameObject creditTextObject; // 📌 À assigner dans l'inspecteur (ton texte de crédits)
    public GameObject parametreMenu;
    public GameObject loadingObject;

    [Header("Camera Animation")]
    public Camera mainCamera;
    public Transform targetPoint;
    public float scrollDuration = 2f;

    private Vector3 initialCameraPosition;
    private Coroutine scrollCoroutine;
    private bool isScrolling = false;

    private void Awake()
    {
        // On cherche le loadingScreen s'il n'est pas assigné
        if (loadingObject == null)
            loadingObject = GameObject.Find("loadingScreen");
    }

    void Start()
    {
        // Position initiale de la caméra
        initialCameraPosition = new Vector3(0, 0, mainCamera.transform.position.z);
        mainCamera.transform.position = initialCameraPosition;

        // Listeners des boutons
        button1.onClick.AddListener(Button1Clicked);
        button2.onClick.AddListener(Button2Clicked);
        button3.onClick.AddListener(Button3Clicked);
        button4.onClick.AddListener(Button4Clicked);
        button5.onClick.AddListener(Button5Clicked);
        button6.onClick.AddListener(Button6Clicked);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenuClicked);

        // Ajout des effets de survol
        AddHoverEffects(button1, hoverTextColor);
        AddHoverEffects(button2, hoverTextColor);
        AddHoverEffects(button3, hoverTextColor);
        AddHoverEffects(button4, hoverTextColor);
        AddHoverEffects(button5, hoverTextColor);
        AddHoverEffects(button6, specialHoverTextColor);

        // Initialisation des états
        if (parametreMenu != null) parametreMenu.SetActive(false);
        if (canvasCredit != null) canvasCredit.SetActive(false);
        if (creditTextObject != null) creditTextObject.SetActive(false);
    }

    void Update()
    {
        // Détection de l'annulation (Echap ou P pour le Web)
        bool cancelPressed = Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P);

        if (cancelPressed && canvasCredit.activeSelf)
        {
            StopCreditSequence();
        }
    }

    // --- LOGIQUE DES BOUTONS ---

    void Button1Clicked() => SoundManager.instance.PlaySFX(sfxClip);

    void Button2Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        StartCoroutine(LoadSceneAsync("SampleScene"));
    }

    void Button3Clicked() => SoundManager.instance.PlaySFX(sfxClip);

    void Button4Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        if (parametreMenu != null) parametreMenu.SetActive(true);
    }

    void ReturnToMenuClicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        if (parametreMenu != null) parametreMenu.SetActive(false);
    }

    void Button5Clicked() // BOUTON CRÉDITS
    {
        SoundManager.instance.PlaySFX(sfxClip);
        if (canvas != null && canvasCredit != null)
        {
            canvas.SetActive(false);
            canvasCredit.SetActive(true);
            scrollCoroutine = StartCoroutine(HandleCreditSequence());
        }
    }

    void Button6Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Application.Quit();
    }

    // --- CRÉDITS & ANIMATION ---

    IEnumerator HandleCreditSequence()
    {
        isScrolling = true;

        // On affiche le texte des crédits
        if (creditTextObject != null) creditTextObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        Vector3 targetPosition = new Vector3(targetPoint.position.x, targetPoint.position.y, mainCamera.transform.position.z);
        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, targetPosition, elapsedTime / scrollDuration);
            elapsedTime += Time.deltaTime;

            if (!isScrolling) yield break;
            yield return null;
        }

        mainCamera.transform.position = targetPosition;

        // Attente à la fin pour laisser lire
        yield return new WaitForSeconds(3f);

        // Fin naturelle
        StopCreditSequence();
    }

    void StopCreditSequence()
    {
        if (scrollCoroutine != null) StopCoroutine(scrollCoroutine);

        isScrolling = false;
        mainCamera.transform.position = initialCameraPosition;

        if (creditTextObject != null) creditTextObject.SetActive(false);
        if (canvasCredit != null) canvasCredit.SetActive(false);
        if (canvas != null) canvas.SetActive(true);
    }

    // --- CHARGEMENT ASYNC ---

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        if (loadingObject != null) loadingObject.SetActive(true);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
    }

    // --- EFFETS UI ---

    void AddHoverEffects(Button button, Color hoverColor)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText == null) return;

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        // Hover Enter
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) =>
        {
            buttonText.color = hoverColor;
            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            buttonText.fontMaterial.SetColor("_OutlineColor", outlineColor);
        });
        trigger.triggers.Add(pointerEnter);

        // Hover Exit
        EventTrigger.Entry pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) =>
        {
            buttonText.color = normalTextColor;
            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0f);
        });
        trigger.triggers.Add(pointerExit);
    }
}