using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Text.RegularExpressions;

public class ShowDialogue : MonoBehaviour
{


    public static ShowDialogue Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }



    

    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] GameObject box;
    private int currentDialogueIndex = 0;
    private bool isTextAnimating = false; 
    private Coroutine typingCoroutine;  

    private Vector2 startSize;
    private Vector3 startScale;

     public RectTransform nextIco;   


    void Start()
    {
        Materials.instance.textDone = true;
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        RectTransform boxRT = box.GetComponent<RectTransform>();
        startSize =  rectTransform.sizeDelta;
        startScale =  boxRT.sizeDelta;
        DontDestroyOnLoad(gameObject);

    }

IEnumerator WaitForFrames(int frameCount, Speech dialogue) //attendre pour éviter le clic accidentel quand le text apparait
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }
        
        StartDialogue(dialogue);
    }


    public void DialogueBox(Speech dialogue)
    {        Materials.instance.canMove = false;
        RectTransform boxRT = box.GetComponent<RectTransform>();
            Debug.Log("test");
        Time.timeScale = 0f;

        RectTransform currentrectTransform = GetComponent<RectTransform>();
        currentrectTransform.anchoredPosition = dialogue.position;
        boxRT.anchoredPosition = new Vector2(boxRT.anchoredPosition.x / 3f, boxRT.anchoredPosition.y);
          currentDialogueIndex = 0;
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        Vector2 vector2Size = new Vector2(dialogue.size.x * 0.3f, dialogue.size.y * 0.3f);
        
        boxRT.sizeDelta = startScale;
        rectTransform.sizeDelta =  startSize;

        rectTransform.sizeDelta += vector2Size;
         Vector3 currentScale = boxRT.sizeDelta;

            boxRT.sizeDelta = currentScale + dialogue.size;

            nextIco.anchoredPosition = new Vector2(nextIco.anchoredPosition.x, nextIco.anchoredPosition.y);

        StartCoroutine(WaitForFrames(5 , dialogue));
    }

    private void StartDialogue(Speech dialogue)
    {
        currentDialogueIndex = 0;
        typingCoroutine = StartCoroutine(TypeText(dialogue.textList[currentDialogueIndex]));
        StartCoroutine(WaitForMouseClick(dialogue));
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


    private IEnumerator WaitForMouseClick(Speech dialogue)
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
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
                        typingCoroutine = StartCoroutine(TypeText(dialogue.textList[currentDialogueIndex]));
                    }
                    else
                    {
        RectTransform currentrectTransform = GetComponent<RectTransform>();
                        currentrectTransform.anchoredPosition = new Vector3(3585, -670, 0);
        textMeshPro.text = "";
        
        Time.timeScale = 1f;
                Materials.instance.canMove = true;
        Materials.instance.textDone  = true;


                        yield break;
                    }
                }
            }

            yield return null;
        }
    }




string ReplacePlaceholders(string text)
    {
        text = text.Replace("{Materials.instance.townName}", Materials.instance.townName);


        return text;
    }
}

