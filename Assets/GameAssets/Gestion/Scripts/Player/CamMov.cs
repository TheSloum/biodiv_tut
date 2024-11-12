using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public KeyCode pauseKey = KeyCode.Space;       // Touche pour mettre en pause
    public KeyCode speedUpKey = KeyCode.P;         // Touche pour accélérer
    public KeyCode resetSpeedKey = KeyCode.M;      // Touche pour réinitialiser

    public float minTimeScale = 1f;  // Vitesse minimale du jeu
    public float maxTimeScale = 4f;  // Vitesse maximale du jeu
    private float currentMultiplier = 1f; // Multiplicateur de vitesse actuel

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }
    
    void LateUpdate()
    {
        HandleMovement();
        HandleCameraDrag();
        HandleZoom();
        HandleKeyboardInput();
    }

    private void HandleMovement()
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
    }

    private void HandleCameraDrag()
    {
        if (Input.GetMouseButton(0))
        {
            if (UIdetection.instance.mouseOverUi){
            return;
        }
            mousPosDif = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position);

            if (!drag)
            {
                drag = true;
                origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            Camera.main.transform.position = origin - mousPosDif;
        }
    }

    private void HandleZoom()
    {
        float scrollData = Input.GetAxis("Mouse ScrollWheel");
        float originalSize = cam.orthographicSize;
        cam.orthographicSize -= scrollData * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        

        

            float zoomFactor = cam.orthographicSize / originalSize;

            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 direction = mouseWorldPos - cam.transform.position;

            cam.transform.position += direction * (1 - zoomFactor);


        Vector3 cameraPos = Camera.main.transform.position;
        float clampedX = Mathf.Clamp(cameraPos.x, minBounds.x, maxBounds.x);
        float clampedY = Mathf.Clamp(cameraPos.y, minBounds.y, maxBounds.y);
        Camera.main.transform.position = new Vector3(clampedX, clampedY, cameraPos.z);
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(speedUpKey))
        {
            FastForward();
        }

        if (Input.GetKeyDown(resetSpeedKey))
        {
            ResetSpeed();
        }
    }

    public void TogglePause()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1; // Reprendre le jeu
        }
        else
        {
            Time.timeScale = 0; // Mettre en pause le jeu
        }
    }

    public void FastForward()
    {
        Time.timeScale = 2; // Reprendre le jeu
    }

    public void ResetSpeed()
    {
        currentMultiplier = 1f; // Réinitialiser
        Time.timeScale = currentMultiplier;
    }
}