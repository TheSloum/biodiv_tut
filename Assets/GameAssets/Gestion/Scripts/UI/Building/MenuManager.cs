using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public RectTransform objectToMove;
public float targetX;
    public Button leftButton;      
    public Button rightButton;    
    public float moveAmount = 280f; 
    public int maxSteps = 10;

    private float minX; 
    private float maxX;
    private bool isMoving = false;

    public float lerpSpeed = 5f;
    
    private Vector2 currentPosition;

    
    void Start()
    {
        minX = 0;
        maxX = moveAmount * maxSteps *-1;

        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);

    }
void Update()
{
if (isMoving)
        {
    currentPosition = objectToMove.anchoredPosition;

    float newX = Mathf.Lerp(currentPosition.x, targetX, Time.deltaTime * lerpSpeed);

    objectToMove.anchoredPosition = new Vector2(newX, currentPosition.y);
        }
        float dif = Mathf.Abs(objectToMove.anchoredPosition.x- targetX);

            if ( dif <= 0.2f)
            {
                objectToMove.anchoredPosition= new Vector2 (targetX, currentPosition.y);
                isMoving = false;
            }




}
    

    void MoveRight()
    {
        if (targetX > maxX)
        {
            targetX -= moveAmount;
            isMoving = true;
        }
    }

    void MoveLeft()
    {
        if (targetX < 0)
        {
            targetX += moveAmount;
            isMoving = true;
        }
    }

}