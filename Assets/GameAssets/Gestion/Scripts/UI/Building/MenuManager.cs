using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public RectTransform objectToMove;
    public AudioClip sfxClip;
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
    private Vector2 initialPosition;

    void Start()
    {
        minX = 0;
        maxX = moveAmount * maxSteps * -1;

        initialPosition = objectToMove.anchoredPosition;

        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);

        ResetPosition();
        UpdateButtonStates();
    }

    void Update()
    {
        if (isMoving)
        {
            currentPosition = objectToMove.anchoredPosition;
            float newX = Mathf.Lerp(currentPosition.x, targetX, Time.unscaledDeltaTime * lerpSpeed);
            objectToMove.anchoredPosition = new Vector2(newX, currentPosition.y);
        }

        float dif = Mathf.Abs(objectToMove.anchoredPosition.x - targetX);

        if (dif <= 0.2f)
        {
            objectToMove.anchoredPosition = new Vector2(targetX, currentPosition.y);
            isMoving = false;
        }

        UpdateButtonStates();
    }

    void MoveRight()
    {
        if (targetX > maxX)
        {
            SoundManager.instance.PlaySFX(sfxClip);
            targetX -= moveAmount;
            isMoving = true;
        }
    }

    void MoveLeft()
    {
        if (targetX < 0)
        {
            SoundManager.instance.PlaySFX(sfxClip);
            targetX += moveAmount;
            isMoving = true;
        }
    }

    void UpdateButtonStates()
    {
        bool hasEnoughChildren = objectToMove.childCount > 3;
        leftButton.gameObject.SetActive(hasEnoughChildren && targetX < 0);
        rightButton.gameObject.SetActive(hasEnoughChildren && targetX > maxX);
    }

    public void ResetPosition()
    {
        objectToMove.anchoredPosition = initialPosition;
        targetX = initialPosition.x;
        UpdateButtonStates();
    }


}
