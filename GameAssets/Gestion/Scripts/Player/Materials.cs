using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public int mat_0 = 500;
    public int mat_1 = 500;
    public int mat_2 = 500;
    public int price = 500;
    public float bar_0 = 0.5f;
    public float bar_1 = 0.5f;
    public float bar_2 = 0.5f;

    [SerializeField] private GameObject bar0; 
    private ProgressBar barMet0;

    [SerializeField] private GameObject bar1; 
    private ProgressBar barMet1;
    
    [SerializeField] private GameObject bar2; 
    private ProgressBar barMet2;
    void Start()
    {
        
        barMet0 = bar0.GetComponent<ProgressBar>();
        barMet1 = bar1.GetComponent<ProgressBar>();
        barMet2 = bar2.GetComponent<ProgressBar>();
        
    }

    void Update()
    {


if (bar_0 < 0f){
    bar_0 = 0f;
}
if (bar_0 > 1f){
    bar_0 = 1f;
}
if (bar_1 < 0f){
    bar_1 = 0f;
}
if (bar_1 > 1f){
    bar_1 = 1f;
}
if (bar_2 < 0f){
    bar_2 = 0f;
}
if (bar_2 > 1f){
    bar_2 = 1f;
}

        barMet0.targetValue = bar_0;
        barMet1.targetValue = bar_1;
        barMet1.targetValue = bar_2;
    }
}
