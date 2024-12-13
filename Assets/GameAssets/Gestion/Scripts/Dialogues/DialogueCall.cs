using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCall : MonoBehaviour
{
    public Speech speechData; 
    public ShowDialogue dialogueManager;

    public void TriggerDialogue()
    {
        if (dialogueManager != null && speechData != null)
        {
            dialogueManager.DialogueBox(speechData);
        }
    }
}

