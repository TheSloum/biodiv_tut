using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMov : MonoBehaviour
{
    private bool drag = false;



    private Vector3 mousPosDif;
    private Vector3 origin;

    public Camera cam;          
    public float zoomSpeed = 10f; 
    public float minZoom = 2f;    
    public float maxZoom = 30f;

    public Vector2 minBounds = new Vector2(-10f, -10f); 
    public Vector2 maxBounds = new Vector2(10f, 10f);

    public float moveSpeed = 5f;       
    public float acceleration = 10f;    
    public float deceleration = 15f;    
    public float maxSpeed = 10f;         
    public float stopDrag = 5f;  

    private Rigidbody2D rb;




    void Awake(){
 rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movementInput = new Vector2(horizontal, vertical).normalized;

        if (movementInput.magnitude > 0)
        {
            rb.AddForce(movementInput * moveSpeed * acceleration * Time.deltaTime);

            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, stopDrag * Time.deltaTime);
        }


    if (Input.GetMouseButton(0))
            {
                
                
                mousPosDif = (Camera.main.ScreenToWorldPoint(Input.mousePosition)- Camera.main.transform.position);

                if (drag == false){
                    drag = true;
                    origin = Camera.main.ScreenToWorldPoint (Input.mousePosition);
                }

            }
            else
            {
                drag = false;
            }

            if(drag==true){
                Camera.main.transform.position = origin - mousPosDif;
            }


            
    

    float scrollData = Input.GetAxis("Mouse ScrollWheel");

        cam.orthographicSize -= scrollData * zoomSpeed;
        Vector3 cameraPos = Camera.main.transform.position;

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        float clampedX = Mathf.Clamp(cameraPos.x, minBounds.x , maxBounds.x);
        float clampedY = Mathf.Clamp(cameraPos.y, minBounds.y, maxBounds.y);

        Camera.main.transform.position = new Vector3(clampedX, clampedY, cameraPos.z);
    }




}
