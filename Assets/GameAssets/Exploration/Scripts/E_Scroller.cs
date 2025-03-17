using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Scroller : MonoBehaviour
{
    public float scrollSpeed = 2f; // Speed at which the background scrolls
    public Sprite[] type1Sprites; // Pool of Type 1 sprites
    public Sprite[] type2Sprites; // Pool of Type 2 sprites
    public Sprite[] type3Sprites; // Pool of Type 3 sprites

    private GameObject currentImage; // Current active image
    private GameObject nextImage; // Image that will appear next
    private int currentType = 1; // Tracks the current type (1, 2, or 3)
    private Vector3 screenBounds; // Screen size in world units

    public int layer = 0;
    public bool fore = false;

    void Start()
    {
        // Get the screen bounds in world units
        Camera mainCamera = Camera.main;
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));

        // Initialize the first image
        currentImage = CreateImageFromSprite(GetRandomSprite(type1Sprites));
        foreach (Sprite sprite in type1Sprites)
    {
        if (sprite != null && sprite.name == "Foreground")
        {
        currentImage.transform.position = new Vector3(0, 0, 0);
        } else {
            
        currentImage.transform.position = new Vector3(0, 0.7f, 0);
        }
    }

        // Create the next image
        CreateNextImage();
    }

    void Update()
    {
        // Scroll the current and next images
        currentImage.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);
        nextImage.transform.Translate(Vector3.left * scrollSpeed * Time.deltaTime);

        // Check if the current image is out of bounds
        if (currentImage.transform.position.x < -screenBounds.x * 2)
        {
            // Destroy the old image and shift references
            Destroy(currentImage);
            currentImage = nextImage;

            // Create a new next image
            CreateNextImage();
        }
    }

    private void CreateNextImage()
    {
        Sprite nextSprite = GetRandomSprite(GetNextSpritePool());
        nextImage = CreateImageFromSprite(nextSprite);

        // Position the next image to the right of the current image
        float imageWidth = currentImage.GetComponent<SpriteRenderer>().bounds.size.x;
        if (nextSprite != null && nextSprite.name == "Foreground")
{
        nextImage.transform.position = new Vector3(currentImage.transform.position.x + imageWidth, 0, 0);
} else {
    
        nextImage.transform.position = new Vector3(currentImage.transform.position.x + imageWidth, 0.7f, 0);
}

        // Update the current type
        currentType = GetNextType();
    }

    private GameObject CreateImageFromSprite(Sprite sprite)
    {
        GameObject imageObject = new GameObject("ScrollingImage");
        SpriteRenderer renderer = imageObject.AddComponent<SpriteRenderer>();
        renderer.sprite = sprite;
        imageObject.transform.localScale = Vector3.one * 3.2f;
        renderer.sortingOrder = layer;
        return imageObject;
    }

    private Sprite GetRandomSprite(Sprite[] pool)
    {
        int randomIndex = Random.Range(0, pool.Length);
        return pool[randomIndex];
    }

    private Sprite[] GetNextSpritePool()
    {
        switch (currentType)
        {
            case 1: return type2Sprites;
            case 2: return type3Sprites;
            case 3: return type1Sprites;
            default: return type1Sprites;
        }
    }

    private int GetNextType()
    {
        switch (currentType)
        {
            case 1: return 2;
            case 2: return 3;
            case 3: return 1;
            default: return 1;
        }
    }

}
