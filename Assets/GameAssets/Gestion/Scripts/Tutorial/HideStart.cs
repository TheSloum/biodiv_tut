using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HideStart : MonoBehaviour
{
    [SerializeField] private GameObject HideGui;
    [SerializeField] private GameObject cameraObject;

    public GameObject pauser;
    public AudioClip sfxClip;
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

    public Building buildRes;
    public GameObject tutoChoosePage;
    public GameObject inputBack;
    [SerializeField] private GameObject dialogueMasking;

    public SpriteRenderer bar0Hide;
    public SpriteRenderer bar1Hide;

    public GameObject Blink1;
    public GameObject Blink1a;
    public GameObject Blink1b;
    public GameObject Blink1c;
    public GameObject Blink2;
    public GameObject Blink3;
    public GameObject Blink4;
    public GameObject Blink5;

    private bool speech3AlreadyDone = false;

    public Builder builder;

    private HashSet<int> shownDialogues = new HashSet<int>(); // To track which dialogues have been shown

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

        inputField.onEndEdit.AddListener(OnEndEdit);
    }

    public void tutoChoose(bool tuto)
    {
        SoundManager.instance.PlaySFX(sfxClip);
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
        SoundManager.instance.PlaySFX(sfxClip);
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
            if (!shownDialogues.Contains(1))
            {
                ShowDialogue.Instance.DialogueBox(speech[1]);
                shownDialogues.Add(1);
            }

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
        if (!shownDialogues.Contains(2))
        {
            ShowDialogue.Instance.DialogueBox(speech[2]);
            shownDialogues.Add(2);
        }
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
        builder.SelectBuild(buildRes, null);
        Materials.instance.textDone = false;
        if (!shownDialogues.Contains(4))
        {
            ShowDialogue.Instance.DialogueBox(speech[4]);
            shownDialogues.Add(4);
        }
        StartCoroutine(WaitForTextEnd(4));
    }
    private void OnEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnSubmit();
        }
    }

    private void OnButtonCloseClicked()
    {
        if (step == 3)
        {
            exploration.interactable = true;
            Materials.instance.textDone = false;
            if (!shownDialogues.Contains(5))
            {
                ShowDialogue.Instance.DialogueBox(speech[5]);
                shownDialogues.Add(5);
            }
            exploration.onClick.AddListener(TutoPart1End);
            Materials.instance.canMove = false;

            StartCoroutine(WaitForTextEnd(5));
        }
        else if (speech3AlreadyDone == false)
        {
            StartCoroutine(FadeInSprites(fadeObject2, fadeDuration));
            Materials.instance.canMove = false;
            Materials.instance.textDone = false;
            if (!shownDialogues.Contains(3))
            {
                ShowDialogue.Instance.DialogueBox(speech[3]);
                shownDialogues.Add(3);
            }
            StartCoroutine(WaitForTextEnd(3));
            StartCoroutine(BlinkGUI());
            speech3AlreadyDone = true;
        }
    }

    private void TutoPart1End()
    {
        // Finish the tutorial part 1 logic here if needed
    }

    private IEnumerator BlinkGUI()
    {
        while (ShowDialogue.Instance.currentDialogueIndex == 0)
        {
            Blink1.SetActive(!Blink1.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }
        while (ShowDialogue.Instance.currentDialogueIndex == 1)
        {
            Blink1.SetActive(true);
            Blink1b.SetActive(false);
            Blink1c.SetActive(false);
            Blink1a.SetActive(!Blink1a.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }
        while (ShowDialogue.Instance.currentDialogueIndex == 2)
        {
            Blink1a.SetActive(false);
            Blink1c.SetActive(false);
            Blink1b.SetActive(!Blink1b.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }
        while (ShowDialogue.Instance.currentDialogueIndex == 3)
        {
            Blink1b.SetActive(false);
            Blink1a.SetActive(false);
            Blink1c.SetActive(!Blink1c.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        while (ShowDialogue.Instance.currentDialogueIndex == 4)
        {
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
            Blink4.SetActive(!Blink4.activeSelf);
            Blink5.SetActive(!Blink5.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink3.SetActive(false);
        Blink4.SetActive(false);
        Blink5.SetActive(false);

        while (ShowDialogue.Instance.currentDialogueIndex == 7)
        {
            Blink3.SetActive(!Blink3.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink3.SetActive(false);

        while (ShowDialogue.Instance.currentDialogueIndex == 8)
        {
            Blink4.SetActive(!Blink4.activeSelf);

            yield return new WaitForSecondsRealtime(0.5f);
        }

        Blink4.SetActive(false);

        while (ShowDialogue.Instance.currentDialogueIndex == 9)
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
