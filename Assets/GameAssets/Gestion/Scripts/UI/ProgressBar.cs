using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float lerpSpeed = 5f;
    public float targetValue = 0f;
    private float currentScale = 0.5f;

    [SerializeField] private int bar_id = 0;

    public Animator barAnim;




    void Awake()
    {
        barAnim.speed=0f;
    }

    void Update()
    {
        Debug.Log(Materials.instance.canMove);
        if(bar_id == 0){
        currentScale = Mathf.Lerp(currentScale, Materials.instance.bar_0, Time.deltaTime * lerpSpeed);
        } else if(bar_id == 1){
        currentScale = Mathf.Lerp(currentScale, Materials.instance.bar_1, Time.deltaTime * lerpSpeed);
        } else{
        currentScale = Mathf.Lerp(currentScale, Materials.instance.bar_2, Time.deltaTime * lerpSpeed);
        }
        barAnim.Play("Bar",0,currentScale);
    }

}