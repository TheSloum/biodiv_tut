using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamMov : MonoBehaviour
{
    public static CamMov Instance { get; private set; }

    private bool drag = false;
    private Vector3 mousPosDif;
    private Vector3 origin;

    [SerializeField] private Camera cam;
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 30f;

    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    private Vector2 minBoundsStart = new Vector2(10f, 10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 10f);
    private Vector2 maxBoundsStart = new Vector2(10f, 10f);
    [SerializeField] private float difXmin = -1402f;
    [SerializeField] private float difYmin = -688f;
    [SerializeField] private float difXmax = 1348f;
    [SerializeField] private float difYmax = 706f;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float stopDrag = 5f;

    private Rigidbody2D rb;

    private KeyCode pauseKey = KeyCode.Space;
    private KeyCode speedUpKey = KeyCode.P;
    private KeyCode resetSpeedKey = KeyCode.M;

    private float currentMultiplier = 1f;

    [Header("UI Buttons")]
    public Button pauseButton;
    public AudioClip sfxClip;
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
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        minBoundsStart = minBounds;
        maxBoundsStart = maxBounds;
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        UpdateButtonSprites();
    }

    void LateUpdate()
    {
        if (Materials.instance.canMove)
        {
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

    public void HandleCameraDrag()
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

        float zoomFactor = 1 - (cam.orthographicSize / maxZoom);

        float zoomMultiplierX = 1f;
        float zoomMultiplierY = 0f;

        float expandedMinX = minBounds.x * (1 + zoomFactor * zoomMultiplierX);
        float expandedMaxX = maxBounds.x * (1 + zoomFactor * zoomMultiplierX);
        float expandedMinY = minBounds.y * (1 + zoomFactor * zoomMultiplierY);
        float expandedMaxY = maxBounds.y * (1 + zoomFactor * zoomMultiplierY);

        Vector3 cameraPos = Camera.main.transform.position;
        float clampedX = Mathf.Clamp(cameraPos.x, expandedMinX, expandedMaxX);
        float clampedY = Mathf.Clamp(cameraPos.y, expandedMinY, expandedMaxY);

        Camera.main.transform.position = new Vector3(clampedX, clampedY, cameraPos.z);
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(pauseKey))
            TogglePause();

        if (Input.GetKeyDown(speedUpKey))
            FastForward();

        if (Input.GetKeyDown(resetSpeedKey))
            ResetSpeed();
    }

    public void Pause()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 0;
        UpdateButtonSprites();
    }

    public void TogglePause()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
        UpdateButtonSprites();
    }

    public void FastForward()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 5;
        UpdateButtonSprites();
    }
    public void ResetZoom()
    {
        if (cam != null)
        {
            // On remet la taille de la caméra sur la valeur maximale (ou une valeur par défaut)
            cam.orthographicSize = maxZoom;

            // Optionnel : On peut aussi recentrer la caméra sur (0,0) si tu veux
            // cam.transform.position = new Vector3(0, 0, cam.transform.position.z);

            Debug.Log("Caméra : Zoom réinitialisé pour l'événement.");
        }
    }
    public void ResetSpeed()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Time.timeScale = 1;
        UpdateButtonSprites();
    }

    private void UpdateButtonSprites()
    {
        pauseButton.image.sprite = Time.timeScale == 0 ? pauseSpriteActive : pauseSpriteInactive;
        pauseUI.SetActive(Time.timeScale == 0);

        playButton.image.sprite = Time.timeScale == 1 ? playSpriteActive : playSpriteInactive;

        speedUpButton.image.sprite = Time.timeScale == 5 ? speedUpSpriteActive : speedUpSpriteInactive;
    }

    void Update()
    {
        gui.SetActive(!IsAnyGameObjectActive(menus));
    }

    private bool IsAnyGameObjectActive(GameObject[] gameObjects)
    {
        foreach (var obj in gameObjects)
            if (obj != null && obj.activeInHierarchy) return true;
        return false;
    }
}