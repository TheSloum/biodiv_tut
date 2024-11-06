using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbuttons : MonoBehaviour
{
    [SerializeField] private GameObject unlockButton ; 
    [SerializeField] private GameObject unlockMenu ;
    void Start()
    {
        
    }

void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject == unlockButton)
                {
                        unlockMenu.SetActive(!unlockMenu.activeSelf);
                }
            }
        }
    }
}
