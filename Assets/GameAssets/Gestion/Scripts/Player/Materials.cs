using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{    public static Materials instance;
    public int mat_0 = 500;
    public int mat_1 = 500;
    public int mat_2 = 500;
    public int price = 500;
    public float bar_0 = 0.5f;
    public float bar_1 = 0.5f;
    public float bar_2 = 0.5f;

private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    void Update()
    {


if (bar_0 < 0f){
    bar_0 = 0f;
}
if (bar_0 >= 1f){
    bar_0 = 0.99f;
}
if (bar_1 < 0f){
    bar_1 = 0f;
}
if (bar_1 >= 1f){
    bar_1 = 0.99f;
}
if (bar_2 < 0f){
    bar_2 = 0f;
}
if (bar_2 >= 1f){
    bar_2 = 0.99f;
}




    }


}
