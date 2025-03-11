using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIdetection : MonoBehaviour
{
    public static UIdetection instance;

    public bool ui_open = false;
    public bool mouseOverUi = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Update()
    {
        mouseOverUi = false;
        
        if (EventSystem.current.IsPointerOverGameObject())
        {
            mouseOverUi = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseOverUi = false;
        }
    }
}
