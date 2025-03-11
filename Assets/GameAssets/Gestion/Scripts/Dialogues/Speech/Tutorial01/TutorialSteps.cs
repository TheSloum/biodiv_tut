using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialSteps : MonoBehaviour
{
    public List<Speech> speech;
    public GameObject buildMenu;


private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}
    
    public IEnumerator TutorialP3(){
        Materials.instance.tutorial = false;
            if (!GameDataSaver.instance.reseachHere){
                        Materials.instance.researchCentr = true;
            }
                    if (Materials.instance.researchCentr){
            yield return new WaitForSecondsRealtime(0.2f);
            ShowDialogue.Instance.DialogueBox(speech[0]);
            while (Materials.instance.researchCentr || buildMenu.activeInHierarchy)
        {
            yield return null; 
        }

            ShowDialogue.Instance.DialogueBox(speech[1]);
                    }
            

    }

     private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if ( Materials.instance.tutorialStep == true){
            if (scene.name == "SampleScene") 
        {
        StartCoroutine(TutorialP3());
        }
        }
    }
}
