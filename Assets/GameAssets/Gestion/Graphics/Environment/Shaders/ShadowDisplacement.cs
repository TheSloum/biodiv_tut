using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDisplacement : MonoBehaviour
{
      public Texture2D heightmap; // Reference to the heightmap (greyscale texture)
    public Sprite cloudSprite; // The cloud shadow sprite
    private SpriteRenderer cloudRenderer;
    private Vector3 cloudOriginalPosition;

    void Start()
    {
        cloudRenderer = GetComponent<SpriteRenderer>();
        cloudOriginalPosition = transform.position; // Save the original position
    }

    void Update()
    {
        AdjustCloudPosition();
    }

    void AdjustCloudPosition()
    {
        // Get the cloud's texture dimensions
        Vector2 cloudSize = cloudRenderer.bounds.size;

        // Loop through every pixel in the cloud shadow's texture
        for (int x = 0; x < cloudSize.x; x++)
        {
            for (int y = 0; y < cloudSize.y; y++)
            {
                // Convert the pixel positions from world space to local space
                Vector3 localPos = transform.InverseTransformPoint(new Vector3(x, y, 0));

                // Sample the heightmap at the position of the current cloud pixel
                Color heightmapColor = heightmap.GetPixelBilinear(localPos.x / heightmap.width, localPos.y / heightmap.height);

                // The intensity of the heightmap's grayscale value (black = low, white = high)
                float heightValue = heightmapColor.grayscale; // Grayscale value between 0 and 1

                // Modify the cloud's position based on the heightmap value
                Vector3 newPosition = cloudOriginalPosition;
                newPosition.y += heightValue * 2.0f; // Scale the height difference as needed
                transform.position = newPosition;
            }
        }
    }
}