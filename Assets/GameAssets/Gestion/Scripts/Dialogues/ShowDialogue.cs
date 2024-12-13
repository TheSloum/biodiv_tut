using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowDialogue : MonoBehaviour
{
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
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        startSize =  rectTransform.sizeDelta;
        startScale =  box.transform.localScale;

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
    {
        RectTransform currentrectTransform = GetComponent<RectTransform>();
        currentrectTransform.anchoredPosition = dialogue.position;
          currentDialogueIndex = 0;
        RectTransform rectTransform = textMeshPro.GetComponent<RectTransform>();
        Vector2 vector2Size = new Vector2(dialogue.size.x * 0.3f, dialogue.size.y * 0.3f);
        
        box.transform.localScale = startScale;
        rectTransform.sizeDelta =  startSize;

        rectTransform.sizeDelta += vector2Size;
         Vector3 currentScale = box.transform.localScale;

            box.transform.localScale = currentScale + dialogue.size;

            nextIco.anchoredPosition = new Vector2(nextIco.anchoredPosition.x, nextIco.anchoredPosition.y - 5);

        StartCoroutine(WaitForFrames(5 , dialogue));
    }

    private void StartDialogue(Speech dialogue)
    {
        // Start displaying the first dialogue
        currentDialogueIndex = 0;
        typingCoroutine = StartCoroutine(TypeText(dialogue.textList[currentDialogueIndex]));
        StartCoroutine(WaitForMouseClick(dialogue));
    }

    private IEnumerator TypeText(string text)
    {
        isTextAnimating = true;
        textMeshPro.text = "";
        nextIco.gameObject.SetActive(false);

        foreach (char letter in text)
        {
            textMeshPro.text += letter;
            yield return new WaitForSeconds(0.03f); 
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
                    // Finish typing instantly
                    StopCoroutine(typingCoroutine);
                    textMeshPro.text = dialogue.textList[currentDialogueIndex];
                    isTextAnimating = false;
        nextIco.gameObject.SetActive(true);
                }
                else
                {
                    // Go to the next dialogue if typing is done
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
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }


}
