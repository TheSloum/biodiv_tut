using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenNo : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
    GameObject loadingObject = GameObject.Find("loadingScreen");
    loadingObject.SetActive(false);
    }

}
