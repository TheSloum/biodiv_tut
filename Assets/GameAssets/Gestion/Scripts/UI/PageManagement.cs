using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManagement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ClosePage(GameObject Page)
    {
        Page.SetActive(false);
    }
    public void OpenPage(GameObject Page){
        Page.SetActive(true);
}
}
