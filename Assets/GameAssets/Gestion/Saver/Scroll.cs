using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scroll : MonoBehaviour
{
    private Vector3 initialPosition;
    public float objectHeight;
    public float scrollSpeed = 50f;

    void Start()
    {
        // Save the initial position of the object
        initialPosition = transform.position;

        // Calculate the max scroll height based on the number of children
        objectHeight = GetObjectHeight();
    }

    void Update()
    {
        // Get the scroll input from the mouse wheel
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            float scrollAmount = scrollInput * scrollSpeed;

            float newYPosition = transform.position.y + scrollAmount;

            newYPosition = Mathf.Clamp(newYPosition, initialPosition.y, initialPosition.y + objectHeight);

            transform.position = new Vector3(transform.position.x, newYPosition, transform.position.z);
        }
    }

    public float GetObjectHeight()
    {
        int numberOfChildren = transform.childCount;
        if (SceneManager.GetActiveScene().name == "SampleScene"){
        return numberOfChildren * 8f;
        }else {
            
        return numberOfChildren * 50f;
        }
    }
}

