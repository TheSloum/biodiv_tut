using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeEffect : MonoBehaviour
{
    public GameObject smok;

    void Update()
    {
        

        Color tmp = smok.GetComponent<SpriteRenderer>().color;
        tmp.a =  Mathf.Clamp(Materials.instance.bar_2, 0f, 0.45f);
        smok.GetComponent<SpriteRenderer>().color = tmp;
        
    }
}
