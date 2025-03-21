using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HideStart : MonoBehaviour
{

    //code le plus brut et sale que j'ai fjamais fait mais j'en branle
    [SerializeField] private GameObject HideGui;
    [SerializeField] private GameObject cameraObject;

    public GameObject pauser;


    public TMP_InputField inputField;
    public Button submitButton;


    public List<Speech> speech;

    public List<Transform> target;
    public float moveSpeed = 5f;

    private bool isMoving = false;

    [SerializeField] private GameObject hideInput;



    public GameObject fadeObject;
    public GameObject fadeObject2;
    public GameObject fadeObject3;


    public float fadeDuration = 2.0f;

    public float disableDuration = 2f;


    public GameObject prebuild1;
    public GameObject prebuild2;
    public GameObject prebuild3;


    public Button unlock1;
    public Button unlock2;
    public Button unlock3;

    public int step = 0;

    public Button exploration;
    public GameObject manageMenu;



    public GameObject tutoChoosePage;
    public GameObject inputBack;
    [SerializeField] private GameObject dialogueMasking;


    public SpriteRenderer bar0Hide;
    public SpriteRenderer bar1Hide;

    public GameObject Blink1;
    public GameObject Blink2;
    public GameObject Blink3;
    public GameObject Blink4;
    public GameObject Blink5;

    void Start()
    {
        if (!Materials.instance.isLoad)
        {
            StartCoroutine(DisableAllButtons());

            gameObject.SetActive(true);
            HideGui.SetActive(false);
            inputField.onValueChanged.AddListener(ValidateInput);
            submitButton.onClick.AddListener(OnSubmit);
            Materials.instance.canMove = false;

        }
        else
        {

            Materials.instance.canMove = true;
            gameObject.SetActive(false);
        }

    }

    public void tutoChoose(bool tuto)
    {

        hideInput.SetActive(false);
        HideGui.SetActive(true);
        if (tuto)
        {

            Materials.instance.textDone = false;
            ShowDialogue.Instance.DialogueBox(speech[0]);
            StartCoroutine(WaitForTextEnd(0));
        }
        else
        {
            Materials.instance.canMove = true;
            Materials.instance.tutorial = false;
        }
        tutoChoosePage.SetActive(false);
        inputBack.SetActive(false);


    }

    public IEnumerator DisableAllButtons()
    {

        CamMov cammov = pauser.GetComponent<CamMov>();
        cammov.TogglePause();
        List<Button> allButtons = new List<Button>();


        foreach (Button button in Resources.FindObjectsOfTypeAll<Button>())
        {
            if (button.gameObject.scene.isLoaded)
            {
                allButtons.Add(button);
            }
        }

        foreach (Button button in allButtons)
        {
            if (button.CompareTag("ButtonTutorial"))
            {
                continue;
            }
            button.interactable = false;
        }

        while (Materials.instance.tutorial)
        {
            yield return null;
        }

        cammov.TogglePause();
        foreach (Button button in allButtons)
        {
            button.interactable = true;
        }
        Materials.instance.ReseachButton(false);
    }


    private void ValidateInput(string input)
    {
        string filteredInput = "";
        foreach (char c in input)
        {
            if (char.IsLetter(c) || c == '-')
            {
                filteredInput += c;
            }
        }

        if (filteredInput != input)
        {
            inputField.text = filteredInput;
        }
    }


    private void OnSubmit()
    {
        string userInput = inputField.text;

        if (!string.IsNullOrEmpty(userInput))
        {
            Materials.instance.townName = userInput;
            Materials.instance.canMove = true;

        }
        tutoChoosePage.SetActive(true);
        hideInput.SetActive(false);
    }

    private IEnumerator MoveToTarget(Transform target, float moveSpeed)
    {
        Materials.instance.canMove = false;

        while (Vector3.Distance(cameraObject.transform.position, target.position) > 0.05f)
        {
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, target.position, Time.unscaledDeltaTime * moveSpeed);
            yield return null;
        }

        cameraObject.transform.position = target.position;

        if (step == 0)
        {
            StartCoroutine(FadeInSprites(fadeObject, fadeDuration));
        }
        else if (step == 1)
        {
            StartCoroutine(FadeInSprites(fadeObject3, fadeDuration));
        }
    }
    private IEnumerator FadeInSprites(GameObject target, float duration)
    {
        if (target == null) yield break;

        SpriteRenderer[] spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>();

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0.0f;
                sr.color = color;
            }
        }

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0.0f, 0.6f, elapsedTime / duration);
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0.6f;
                sr.color = color;
            }
        }

        if (step == 0)
        {
            Materials.instance.textDone = false;
            ShowDialogue.Instance.DialogueBox(speech[1]);

            Color colorBar0 = bar0Hide.color;
            Color colorBar1 = bar1Hide.color;
            colorBar0.a = 0f;
            colorBar1.a = 0f;

            bar0Hide.color = colorBar0;
            bar1Hide.color = colorBar1;
            dialogueMasking.SetActive(true);
            StartCoroutine(WaitForTextEnd(1));
            step = 1;
        }
        else if (step == 1)
        {
            StartCoroutine(CheckForClicks());
        }
    }


    private IEnumerator FadeOutSprites(GameObject target, float duration)
    {
        if (target == null) yield break;

        SpriteRenderer[] spriteRenderers = target.GetComponentsInChildren<SpriteRenderer>();

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(0.6f, 0.0f, elapsedTime / duration);

            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }
            }

            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0.0f;
                sr.color = color;
            }
        }
    }



    private IEnumerator CheckForClicks()
    {

        while (true)
        {
            yield return null;

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if (prebuild1.GetComponent<Collider2D>() == Physics2D.OverlapPoint(mousePos))
                {
                    yield return new WaitForSecondsRealtime(0.2f);
                    OnObjectClick();
                    yield break;
                }

                if (prebuild2.GetComponent<Collider2D>() == Physics2D.OverlapPoint(mousePos))
                {
                    yield return new WaitForSecondsRealtime(0.2f);
                    OnObjectClick();
                    yield break;
                }

                if (prebuild3.GetComponent<Collider2D>() == Physics2D.OverlapPoint(mousePos))
                {
                    yield return new WaitForSecondsRealtime(0.2f);
                    OnObjectClick2();
                    yield break;
                }
            }
        }
    }
    void OnObjectClick()
    {
        SpriteRenderer[] spriteRenderers = fadeObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }
        Materials.instance.textDone = false;
        ShowDialogue.Instance.DialogueBox(speech[2]);
        StartCoroutine(WaitForTextEnd(2));
    }


    void OnObjectClick2()
    {
        SpriteRenderer[] spriteRenderers = fadeObject3.GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }
        Materials.instance.textDone = false;
        ShowDialogue.Instance.DialogueBox(speech[4]);
        StartCoroutine(WaitForTextEnd(4));
    }




    private void OnButtonCloseClicked()
    {
        if (step == 3)
        {
            exploration.interactable = true;

            Materials.instance.textDone = false;
            ShowDialogue.Instance.DialogueBox(speech[5]);
            exploration.onClick.AddListener(TutoPart1End);
            Materials.instance.canMove = false;

            StartCoroutine(WaitForTextEnd(5));
        }
        else
        {
            StartCoroutine(FadeInSprites(fadeObject2, fadeDuration));
            Materials.instance.canMove = false;
            Materials.instance.textDone = false;
            ShowDialogue.Instance.DialogueBox(speech[3]);
            StartCoroutine(WaitForTextEnd(3));
            StartCoroutine(BlinkGUI());
        }

    }

    private void TutoPart1End()
    {

    }

    private IEnumerator BlinkGUI()
    {
        while (ShowDialogue.Instance.currentDialogueIndex <= 4)
        {
            Blink1.SetActive(!Blink1.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink1.SetActive(false);

        while (ShowDialogue.Instance.currentDialogueIndex == 5)
        {
            Blink2.SetActive(!Blink2.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink2.SetActive(false);


        while (ShowDialogue.Instance.currentDialogueIndex == 6)
        {
            Blink3.SetActive(!Blink3.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink3.SetActive(false);


        while (ShowDialogue.Instance.currentDialogueIndex == 7)
        {
            Blink4.SetActive(!Blink4.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink4.SetActive(false);



        while (ShowDialogue.Instance.currentDialogueIndex == 8)
        {
            Blink5.SetActive(!Blink5.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink5.SetActive(false);


    }
    private IEnumerator WaitForTextEnd(int index)
    {
        yield return new WaitUntil(() => Materials.instance.textDone == true);
        if (index == 1)
        {
            Color colorBar0 = bar0Hide.color;
            Color colorBar1 = bar1Hide.color;

            colorBar0.a = 1f;
            colorBar1.a = 1f;

            bar0Hide.color = colorBar0;
            bar1Hide.color = colorBar1;

            dialogueMasking.SetActive(false);
        }
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.2f);
        if (index == 0)
        {
            StartCoroutine(MoveToTarget(target[0], moveSpeed));
        }
        else if (index == 1)
        {
            Materials.instance.tutoToggle = true;

            Materials.instance.canMove = false;



            StartCoroutine(CheckForClicks());
        }
        else if (index == 2)
        {
            Materials.instance.canMove = false;

            Materials.instance.tutoToggle = false;
            unlock2.interactable = true;
            unlock2.onClick.AddListener(OnButtonCloseClicked);

        }
        else if (index == 3)
        {
            StartCoroutine(FadeOutSprites(fadeObject2, fadeDuration));
            fadeObject.SetActive(false);
            fadeObject3.SetActive(true);
            StartCoroutine(MoveToTarget(target[1], moveSpeed));
            Materials.instance.tutoToggle = true;
            Materials.instance.canMove = false;
        }
        else if (index == 4)
        {
            Materials.instance.tutoToggle = false;
            Materials.instance.canMove = false;
            unlock3.interactable = true;
            step = 3;
            unlock3.onClick.AddListener(OnButtonCloseClicked);
        }

    }




}
