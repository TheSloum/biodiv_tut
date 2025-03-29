using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class ShowDialogue : MonoBehaviour
{
    public static ShowDialogue Instance { get; private set; }

    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] GameObject box;
    [SerializeField] RectTransform nextIco;
    
    // Référence au fond de dialogue (à assigner dans l'inspecteur)
    public GameObject fondDialog;
    // Référence au GUI à désactiver pendant le dialogue (à assigner dans l'inspecteur)
    public GameObject gui;

    public int currentDialogueIndex = 0;
    private bool isTextAnimating = false;
    private Coroutine typingCoroutine;
    private Vector2 startSize;
    private Vector3 startScale;

    public SpriteRenderer currentSprite;
    public GameObject character;
    public float bobHeight = 1f;
    public float bobSpeed = 3f;
    private Vector3 originalPosition;

    // Flag indiquant si le dialogue en cours est un dialogue d'event
    private bool currentDialogueIsEvent = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }

    void Start()
    {
        Materials.instance.textDone = true;
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        RectTransform boxRT = box.GetComponent<RectTransform>();
        startSize = rectTransform.sizeDelta;
        startScale = boxRT.sizeDelta;
        DontDestroyOnLoad(gameObject);
        originalPosition = character.transform.localPosition;
    }

    private IEnumerator WaitForFrames(int frameCount, Speech dialogue)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
        StartDialogue(dialogue);
    }

    IEnumerator BobUpAndDown()
    {
        float elapsedTime = 0f;
        Vector3 targetPosition = originalPosition + Vector3.up * bobHeight;

        while (elapsedTime < 1f / bobSpeed)
        {
            character.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime * bobSpeed);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        character.transform.localPosition = targetPosition;

        elapsedTime = 0f;
        while (elapsedTime < 1f / bobSpeed)
        {
            character.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime * bobSpeed);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        character.transform.localPosition = originalPosition;
    }

    /// <summary>
    /// Ouvre la boîte de dialogue.
    /// Le paramètre isEventDialogue permet d'indiquer s'il s'agit d'un dialogue déclenché par un event.
    /// </summary>
    public void DialogueBox(Speech dialogue, bool isEventDialogue = false)
    {
        currentDialogueIsEvent = isEventDialogue;
        Materials.instance.canMove = false;

        // Si c'est un dialogue d'event, on affiche le fond en 100% et on désactive le GUI
        if (currentDialogueIsEvent)
        {
            SetFondDialogOpacity(1f);
            if (gui != null)
                gui.SetActive(false);
        }

        RectTransform boxRT = box.GetComponent<RectTransform>();
        Time.timeScale = 0f;

        RectTransform currentRectTransform = GetComponent<RectTransform>();
        currentRectTransform.anchoredPosition = dialogue.position;
        currentDialogueIndex = 0;

        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        Vector2 vector2Size = new Vector2(dialogue.size.x * 0.3f, dialogue.size.y * 0.3f);
        rectTransform.pivot = new Vector2(0, 1);
        boxRT.pivot = new Vector2(0, 1);

        boxRT.sizeDelta = startScale;
        rectTransform.sizeDelta = startSize;

        rectTransform.sizeDelta += vector2Size;
        Vector3 currentScale = boxRT.sizeDelta;
        boxRT.sizeDelta = currentScale + dialogue.size;

        nextIco.anchoredPosition = new Vector2(nextIco.anchoredPosition.x, nextIco.anchoredPosition.y);

        StartCoroutine(WaitForFrames(5, dialogue));
    }

    private void StartDialogue(Speech dialogue)
    {
        currentDialogueIndex = 0;
        StartCoroutine(BobUpAndDown());
        if (dialogue.spriteList != null && dialogue.spriteList.Count > 0)
        {
            currentSprite.sprite = dialogue.spriteList[currentDialogueIndex];
        }
        typingCoroutine = StartCoroutine(TypeText(dialogue.textList[currentDialogueIndex]));
        StartCoroutine(WaitForInput(dialogue));
    }

    private IEnumerator TypeText(string text)
    {
        string textVar = ReplacePlaceholders(text);
        isTextAnimating = true;
        textMeshPro.text = "";
        nextIco.gameObject.SetActive(false);

        string currentText = "";
        int i = 0;
        while (i < textVar.Length)
        {
            if (textVar[i] == '<')
            {
                int endTagIndex = textVar.IndexOf('>', i);
                if (endTagIndex > i)
                {
                    currentText += textVar.Substring(i, endTagIndex - i + 1);
                    i = endTagIndex + 1;
                    continue;
                }
            }

            currentText += textVar[i];
            textMeshPro.text = currentText;
            yield return new WaitForSecondsRealtime(0.03f);
            i++;
        }

        isTextAnimating = false;
        nextIco.gameObject.SetActive(true);
    }

    private IEnumerator WaitForInput(Speech dialogue)
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || 
                Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter))
            {
                if (isTextAnimating)
                {
                    StopCoroutine(typingCoroutine);
                    textMeshPro.text = ReplacePlaceholders(dialogue.textList[currentDialogueIndex]);
                    isTextAnimating = false;
                    nextIco.gameObject.SetActive(true);
                }
                else
                {
                    currentDialogueIndex++;
                    if (currentDialogueIndex < dialogue.textList.Count)
                    {
                        StartCoroutine(BobUpAndDown());
                        if (dialogue.spriteList != null && dialogue.spriteList.Count > currentDialogueIndex)
                        {
                            currentSprite.sprite = dialogue.spriteList[currentDialogueIndex];
                        }
                        typingCoroutine = StartCoroutine(TypeText(dialogue.textList[currentDialogueIndex]));
                    }
                    else
                    {
                        CloseDialogueBox();
                        yield break;
                    }
                }
            }
            yield return null;
        }
    }

    private void CloseDialogueBox()
    {
        RectTransform currentRectTransform = GetComponent<RectTransform>();
        currentRectTransform.anchoredPosition = new Vector3(3585, -670, 0);
        textMeshPro.text = "";
        Time.timeScale = 1f;
        Materials.instance.canMove = true;
        Materials.instance.textDone = true;
        
        // Si c'était un dialogue d'event, on remet le fond à 0% et on réactive le GUI
        if (currentDialogueIsEvent)
        {
            SetFondDialogOpacity(0f);
            if (gui != null)
                gui.SetActive(true);
            currentDialogueIsEvent = false;
        }
    }

    // Méthode qui change l'opacité du fond de dialogue (on suppose ici un SpriteRenderer)
    private void SetFondDialogOpacity(float opacity)
    {
        if (fondDialog != null)
        {
            SpriteRenderer sr = fondDialog.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = opacity;
                sr.color = c;
            }
        }
    }

    string ReplacePlaceholders(string text)
    {
        return text.Replace("{Materials.instance.townName}", Materials.instance.townName);
    }
}
