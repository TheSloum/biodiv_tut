using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_TutoEnter : MonoBehaviour
{
    
    public List<Speech> speech;
    public GameObject oxygenObject;
    public bool e_tutorial = true;

    private int step = 0;

    [SerializeField] private GameObject oxygenBar;
    void Start()
    {
        if(Materials.instance.tutorial){
        oxygenBar.SetActive(false);
        if(Materials.instance.tutorial){
        Time.timeScale = 0f;
        ShowDialogue.Instance.DialogueBox(speech[0]);
        StartCoroutine(WaitForTextEnd());
        }
        E_OxygenManager oxygenManager = oxygenObject.GetComponent<E_OxygenManager>();
    StartCoroutine(tutorialPeriods());
    oxygenManager.maxOxygen = 50000f;
    oxygenManager.currentOxygen = 50000f;
    Materials.instance.tutorialStep = true;
        }
    }



public IEnumerator tutorialPeriods()
{
    while (e_tutorial)
    {
        E_OxygenBubble[] oxygenBubbles = FindObjectsOfType<E_OxygenBubble>();

        foreach (E_OxygenBubble bubble in oxygenBubbles)
        {
            Destroy(bubble.gameObject);
        }
        E_Trash[] trash = FindObjectsOfType<E_Trash>();

        foreach (E_Trash itemtrash in trash)
        {
            Destroy(itemtrash.gameObject);
        }
        
        
        E_Material mat = FindObjectOfType<E_Material>();
        if (mat != null && step == 0) {
            step = 1;
            StartCoroutine(MaterialWait());
        }
        yield return null;
    }
}
private IEnumerator MaterialWait()
{
    
    
        E_OxygenManager oxygenManager = oxygenObject.GetComponent<E_OxygenManager>();
            yield return new WaitForSeconds(1f); 
        ShowDialogue.Instance.DialogueBox(speech[1]);

        while(Materials.instance.sessionWood + Materials.instance.sessionStone + Materials.instance.sessionIron == 0){
            
        oxygenBar.SetActive(true);
            
        yield return null;
        }
        ShowDialogue.Instance.DialogueBox(speech[2]); e_tutorial = false;
    oxygenManager.maxOxygen = 100f;
            
        while(Materials.instance.sessionWood + Materials.instance.sessionStone + Materials.instance.sessionIron < 100){
            
        E_Trash trashsea = FindObjectOfType<E_Trash>();
        if(trashsea != null && step == 1){
            yield return new WaitForSeconds(2.8f); 
        ShowDialogue.Instance.DialogueBox(speech[3]);
        step = 2;
        }
        if (oxygenManager.currentOxygen < 20f){
            oxygenManager.currentOxygen = 20f;
        }
        yield return null;
        }
        
}

    private IEnumerator WaitForTextEnd()
    {
        yield return new WaitUntil(() => Materials.instance.textDone == true);
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1f;
    }
}
