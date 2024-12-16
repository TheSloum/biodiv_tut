using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolluBar : MonoBehaviour
{
    [SerializeField] private Transform targetTransform; 
[SerializeField] private float lerpSpeed = 5f; 
[SerializeField] private GameObject barFill; 
private Material mat;

[SerializeField] private List<Sprite> fishPol;
[SerializeField] private GameObject fishBar;
private SpriteRenderer fishSprite;

void Awake(){
    mat = barFill.GetComponent<Renderer>().material;
    fishSprite = fishBar.GetComponent<SpriteRenderer>();;
}
void Update()
{
mat.SetFloat("_Bar2", Materials.instance.bar_2);
    float targetValue = Mathf.Clamp01(Materials.instance.bar_2);

    float newX = Mathf.Lerp(-197f, 0f, targetValue);

    Vector3 currentPosition = targetTransform.localPosition;
    currentPosition.x = Mathf.Lerp(currentPosition.x, newX, Time.deltaTime * lerpSpeed);
    targetTransform.localPosition = currentPosition;

    if(Materials.instance.bar_2 <= 0.25){
        fishSprite.sprite = fishPol[0];
    } else if(Materials.instance.bar_2 <=0.5){
        fishSprite.sprite = fishPol[1];        
    }else if(Materials.instance.bar_2 <=0.75){
        fishSprite.sprite = fishPol[2];        
    } else {
        fishSprite.sprite = fishPol[3];
    }
}

}
