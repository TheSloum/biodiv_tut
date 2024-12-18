using System.Collections;
using System.Collections.Generic;
using TMPro; // Import if using TextMeshPro
using UnityEngine;
using UnityEngine.UI;

public class HideStart : MonoBehaviour
{
    [SerializeField] private GameObject HideGui;
    [SerializeField] private GameObject cameraObject;


    public TMP_InputField inputField;
    public Button submitButton;


    public Speech speech;

    public List<Transform> target;
    public float moveSpeed = 5f;

    private bool isMoving = false;

    [SerializeField] private GameObject hideInput;


    void Start()
    {
        if (!Materials.instance.isLoad)
        {

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
            hideInput.SetActive(false);
            HideGui.SetActive(true);
            Materials.instance.canMove = true;

            ShowDialogue.Instance.DialogueBox(speech);
            StartCoroutine(WaitForTextEnd());

        }
    }

    private IEnumerator MoveToTarget(Transform target, float moveSpeed)
    {
        Materials.instance.canMove = false;
        while (Vector3.Distance(cameraObject.transform.position, target.position) > 0.05f)
        {
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, target.position, Time.deltaTime * moveSpeed);
            yield return null;
        }

        cameraObject.transform.position = target.position;


    }



    private IEnumerator WaitForTextEnd()
    {
        yield return new WaitUntil(() => Materials.instance.textDone == true);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(MoveToTarget(target[0], moveSpeed));
    }
}
