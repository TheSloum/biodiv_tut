using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public RectTransform barRectTransform; 
    public float targetScaleMultiplier = 18.15f; 
    public float lerpSpeed = 5f;
    public float targetValue = 0f;
    private Vector3 targetScale;
    private Vector3 currentScale;

    void Update()
    {
        targetScale = new Vector3(barRectTransform.localScale.x, targetValue * targetScaleMultiplier, barRectTransform.localScale.z);

        currentScale = Vector3.Lerp(barRectTransform.localScale, targetScale, Time.deltaTime * lerpSpeed);

        barRectTransform.localScale = currentScale;
    }

    public void SetTargetValue(float newValue)
    {
        targetValue = Mathf.Clamp01(newValue);
    }
}