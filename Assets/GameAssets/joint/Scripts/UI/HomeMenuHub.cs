using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HomeMenuHub : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;

    public Color normalTextColor = Color.black;
    public Color hoverTextColor = Color.blue;
    public Color outlineColor = Color.white; 

    void Start()
    {
        button1.onClick.AddListener(Button1Clicked);
        button2.onClick.AddListener(Button2Clicked);
        button3.onClick.AddListener(Button3Clicked);
        button4.onClick.AddListener(Button4Clicked);
        button5.onClick.AddListener(Button5Clicked);
        button6.onClick.AddListener(Button6Clicked);

        AddHoverEffects(button1);
        AddHoverEffects(button2);
        AddHoverEffects(button3);
        AddHoverEffects(button4);
        AddHoverEffects(button5);
        AddHoverEffects(button6);
    }

    void Button1Clicked() { Debug.Log("Button 1 clicked!"); }
    void Button2Clicked() { Debug.Log("Button 2 clicked!"); }
    void Button3Clicked() { Debug.Log("Button 3 clicked!"); }
    void Button4Clicked() { Debug.Log("Button 4 clicked!"); }
    void Button5Clicked() { Debug.Log("Button 5 clicked!"); }
    void Button6Clicked() { Application.Quit(); }
    void AddHoverEffects(Button button)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        pointerEnter.callback.AddListener((eventData) =>
        {
            buttonText.color = hoverTextColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0.2f);
            buttonText.fontMaterial.SetColor("_OutlineColor", outlineColor);
        });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        pointerExit.callback.AddListener((eventData) =>
        {

            buttonText.color = normalTextColor;

            buttonText.fontMaterial.SetFloat("_OutlineWidth", 0f);
        });
        trigger.triggers.Add(pointerExit);
    }
}
