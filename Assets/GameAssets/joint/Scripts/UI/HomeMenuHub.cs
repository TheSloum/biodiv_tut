using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class HomeMenuHub : MonoBehaviour
{
    public AudioClip sfxClip;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button returnToMenuButton;

    public Color normalTextColor = Color.black;
    public Color hoverTextColor = Color.blue;
    public Color specialHoverTextColor = Color.red;
    public Color outlineColor = Color.white;

    public GameObject canvas;
    public GameObject canvasCredit;
    public Camera mainCamera;
    public Transform targetPoint;

    public GameObject parametreMenu; // 📌 GameObject du menu des paramètres

    public float scrollDuration = 2f;

    private Vector3 initialCameraPosition;
    private Coroutine scrollCoroutine;
    private bool isScrolling = false;


    public GameObject loadingObject;
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


    private void Awake()
    {
        loadingObject = GameObject.Find("loadingScreen");
    }

    void Start()
    {
        mainCamera.transform.position = Vector3.zero;

        initialCameraPosition = mainCamera.transform.position;

        button1.onClick.AddListener(Button1Clicked);
        button2.onClick.AddListener(Button2Clicked);
        button3.onClick.AddListener(Button3Clicked);
        button4.onClick.AddListener(Button4Clicked);
        button5.onClick.AddListener(Button5Clicked);
        button6.onClick.AddListener(Button6Clicked);

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.AddListener(ReturnToMenuClicked);
        }
        else
        {
            Debug.LogWarning("[HomeMenuHub] ⚠ Bouton ReturnToMenu non assigné !");
        }

        AddHoverEffects(button1, hoverTextColor);
        AddHoverEffects(button2, hoverTextColor);
        AddHoverEffects(button3, hoverTextColor);
        AddHoverEffects(button4, hoverTextColor);
        AddHoverEffects(button5, hoverTextColor);
        AddHoverEffects(button6, specialHoverTextColor);

        if (parametreMenu != null)
        {
            parametreMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("[HomeMenuHub] ⚠ ERREUR: ParametreMenu n'est pas assigné dans l'Inspector !");
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canvasCredit.activeSelf)
        {
            if (isScrolling && scrollCoroutine != null)
            {
                StopCoroutine(scrollCoroutine);
                isScrolling = false;
            }

            mainCamera.transform.position = initialCameraPosition;
            canvasCredit.SetActive(false);
            canvas.SetActive(true);
        }
    }

    void Button1Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
    }

    void Button2Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        StartCoroutine(LoadSceneAsync("SampleScene"));
    }

    void Button3Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
    }

    void Button4Clicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        if (parametreMenu != null)
        {
            parametreMenu.SetActive(true);
        }
        else
        {
            Debug.LogError("[HomeMenuHub] ⚠ ERREUR: ParametreMenu n'est pas assigné !");
        }
    }

    void ReturnToMenuClicked()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        if (parametreMenu != null)
        {
            parametreMenu.SetActive(false);
        }
        else
        {
            Debug.LogError("[HomeMenuHub] ⚠ ERREUR: ParametreMenu n'est pas assigné !");
        }
    }

    void Button5Clicked()
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

    IEnumerator HandleCreditSequence()
    {
        isScrolling = true;

        yield return new WaitForSeconds(2f);

        Vector3 targetPosition = new Vector3(targetPoint.position.x, targetPoint.position.y, mainCamera.transform.position.z);

        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            mainCamera.transform.position = Vector3.Lerp(initialCameraPosition, targetPosition, elapsedTime / scrollDuration);
            elapsedTime += Time.deltaTime;
            yield return null;

            if (!isScrolling) yield break;
        }

        mainCamera.transform.position = targetPosition;

        yield return new WaitForSeconds(2f);

        mainCamera.transform.position = initialCameraPosition;

        canvasCredit.SetActive(false);
        canvas.SetActive(true);

        isScrolling = false;
    }

    void AddHoverEffects(Button button, Color hoverColor)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) =>
        {
            buttonText.color = hoverColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            buttonText.fontMaterial.SetColor("_OutlineColor", outlineColor);
        });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) =>
        {
            buttonText.color = normalTextColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0f);
        });
        trigger.triggers.Add(pointerExit);
    }
}
