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

        startSize = rectTransform.sizeDelta;

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
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        character.transform.localPosition = targetPosition;

        elapsedTime = 0f;
        while (elapsedTime < 1f / bobSpeed)
        {
            character.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime * bobSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        character.transform.localPosition = originalPosition;
    }


    public void DialogueBox(Speech dialogue)
    {
        Materials.instance.canMove = false;
        RectTransform boxRT = box.GetComponent<RectTransform>();
        Time.timeScale = 0f;


        RectTransform currentrectTransform = GetComponent<RectTransform>();
        currentrectTransform.anchoredPosition = dialogue.position;
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
        currentSprite.sprite = dialogue.spriteList[currentDialogueIndex];
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
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
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
                        currentSprite.sprite = dialogue.spriteList[currentDialogueIndex];
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
        RectTransform currentrectTransform = GetComponent<RectTransform>();
        currentrectTransform.anchoredPosition = new Vector3(3585, -670, 0);
        textMeshPro.text = "";
        Time.timeScale = 1f;
        Materials.instance.canMove = true;
        Materials.instance.textDone = true;
    }

    string ReplacePlaceholders(string text)
    {
        return text.Replace("{Materials.instance.townName}", Materials.instance.townName);
    }
}