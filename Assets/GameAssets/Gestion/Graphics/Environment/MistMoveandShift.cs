using UnityEngine;

public class InfiniteScrollSprite : MonoBehaviour
{
    [Header("Movement Settings")]
    public float scrollSpeed = 1f;              
    public float bobbingAmplitude = 0.5f;       
    public float bobbingFrequency = 1f;        

    [Header("Cloning Settings")]
    public float spawnX = 10f;              
    public float destroyX = 12f; 
    public Transform parentObject;             

    private Vector3 localStartPos;             
    private bool hasSpawnedClone = false;    


private SpriteRenderer spriteRenderer;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        localStartPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition += Vector3.right * scrollSpeed * Time.deltaTime;

        float newY = localStartPos.y + Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);

        if (!hasSpawnedClone && transform.localPosition.x >= spawnX)
        {
            Vector3 cloneLocalPos = new Vector3(-6916f, localStartPos.y, transform.localPosition.z);
            GameObject clone = Instantiate(gameObject, parentObject);

            clone.transform.localPosition = cloneLocalPos;
            clone.transform.localScale = transform.localScale;
            hasSpawnedClone = true;
        }

        if (transform.localPosition.x >= destroyX)
        {
            Destroy(gameObject);
        }
        

         float barValue = Materials.instance.bar_2;

        float normalized = Mathf.Clamp01((barValue - 0.2f) / 0.8f);

        float targetAlpha = normalized * 0.75f;

        Color currentColor = spriteRenderer.color;
        currentColor.a = targetAlpha;
        spriteRenderer.color = currentColor;
    }
}
