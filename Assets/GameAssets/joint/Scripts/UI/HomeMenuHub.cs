using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuHub : MonoBehaviour
{
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button button7;

    // Start is called before the first frame update
    void Start()
    {
        button1.onClick.AddListener(Button1Clicked);
        button2.onClick.AddListener(Button2Clicked);
        button3.onClick.AddListener(Button3Clicked);
        button4.onClick.AddListener(Button4Clicked);
        button5.onClick.AddListener(Button5Clicked);
        button6.onClick.AddListener(Button6Clicked);
        button7.onClick.AddListener(Button7Clicked);
    }

    void Button1Clicked()
    {
        Debug.Log("Button 1 clicked!");
    }

    void Button2Clicked()
    {
        Debug.Log("Button 2 clicked!");
    }

    void Button3Clicked()
    {
        Debug.Log("Button 3 clicked!");
    }
    void Button4Clicked()
    {
        Debug.Log("Button 4 clicked!");
    }
    void Button5Clicked()
    {
        Debug.Log("Button 5 clicked!");
    }
    void Button6Clicked()
    {
        Debug.Log("Button 6 clicked!");
    }
    void Button7Clicked()
    {
        Application.Quit();
    }

}
