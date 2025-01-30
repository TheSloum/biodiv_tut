using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamMov : MonoBehaviour
{
    private bool drag = false;
    private Vector3 mousPosDif;
    private Vector3 origin;

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 30f;

    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float stopDrag = 5f;

    private Rigidbody2D rb;

    // Clés pour contrôle de vitesse
    private KeyCode pauseKey = KeyCode.Space;
    private KeyCode speedUpKey = KeyCode.P;
    private KeyCode resetSpeedKey = KeyCode.M;

    private float currentMultiplier = 1f;

    // UI et Sprites
    [Header("UI Buttons")]
    public Button pauseButton;
    public Button playButton;
    public Button speedUpButton;

    [Header("Sprites")]
    public Sprite pauseSpriteActive;
    public Sprite pauseSpriteInactive;
    public Sprite playSpriteActive;
    public Sprite playSpriteInactive;
    public Sprite speedUpSpriteActive;
    public Sprite speedUpSpriteInactive;
    public GameObject pauseUI;

    [Header("GUI control")]
    public GameObject[] menus; 
    public GameObject gui;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        UpdateButtonSprites();
    }

    void LateUpdate()
    {
        if (Materials.instance.canMove){
        HandleMovement();
        HandleCameraDrag();
        HandleZoom();
        HandleKeyboardInput();
        }
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
        cam.orthographicSize -= scrollData * zoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

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
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        UpdateButtonSprites();
    }

    public void FastForward()
    {
        Time.timeScale = 4;
        UpdateButtonSprites();
    }

    public void ResetSpeed()
    {
        Time.timeScale = 1;
        UpdateButtonSprites();
    }

    private void UpdateButtonSprites()
    {

        if (Time.timeScale == 0)
        {
            pauseButton.image.sprite = pauseSpriteActive;
            pauseUI.SetActive(true);
        }
        else
        {
            pauseButton.image.sprite = pauseSpriteInactive;
            pauseUI.SetActive(false);
        }

        if (Time.timeScale == 1)
        {
            playButton.image.sprite = playSpriteActive;

        }
        else
        {
            playButton.image.sprite = playSpriteInactive;
        }

        if (Time.timeScale == 4)
        {
            speedUpButton.image.sprite = speedUpSpriteActive;
        }
        else
        {
            speedUpButton.image.sprite = speedUpSpriteInactive;
        }
    }

    void Update()
    {
        if (IsAnyGameObjectActive(menus))
        {
            gui.SetActive(false);
        }
        else
        {
            gui.SetActive(true); 
        }
    }

    private bool IsAnyGameObjectActive(GameObject[] gameObjects)
    {
        foreach (var obj in gameObjects)
        {
            if (obj != null && obj.activeInHierarchy)
            {
                return true; 
            }
        }
        return false;
    }
}
