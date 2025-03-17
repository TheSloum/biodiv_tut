using UnityEngine;

public class FishMouvement : MonoBehaviour
{
    public float speed = 600f;
    public float swimAmplitude = 200f;
    public float swimFrequency = 4f;

    private Vector2 screenBounds;
    private Vector3 direction;
    private float originalYPosition;

    void Start()
    {
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        originalYPosition = transform.position.y;
        SetRandomDirection();
        SetStartPosition();
    }

    void Update()
    {
        //horizontal
        transform.Translate(direction * speed * Time.deltaTime);

        //verticale
        float oscillation = Mathf.Sin(Time.time * swimFrequency) * swimAmplitude;
        transform.position = new Vector3(transform.position.x, originalYPosition + oscillation, transform.position.z);

        // Rotation
        AdjustOrientation();

        if (IsFishOffScreen())
        {
            gameObject.SetActive(false);
        }
    }

    void SetRandomDirection()
    {
        direction = (Random.value > 0.5f) ? Vector3.right : Vector3.left;
        Vector3 scale = transform.localScale;

        if (direction == Vector3.right)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    void SetStartPosition()
    {
        float yPosition = Random.Range(-screenBounds.y + 1f, screenBounds.y - 1f);

        if (direction == Vector3.right)
        {
            transform.position = new Vector3(-screenBounds.x - 1f, yPosition, 0);
        }
        else
        {
            transform.position = new Vector3(screenBounds.x + 1f, yPosition, 0);
        }
    }

    void AdjustOrientation()
    {
        float verticalDirection = Mathf.Sin(Time.time * swimFrequency);
        float angle = Mathf.Lerp(-14f, 14f, (verticalDirection + 1f) / 2f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    bool IsFishOffScreen()
    {
        Vector3 fishPos = Camera.main.WorldToViewportPoint(transform.position);

        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width / 2;
        float height = rectTransform.rect.height / 2;


        return fishPos.x < -width || fishPos.x > 1 + width || fishPos.y < -height || fishPos.y > 1 + height;
    }
}
