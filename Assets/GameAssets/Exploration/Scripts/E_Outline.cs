using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Outline : MonoBehaviour
{
    
    public Color outlineColor = Color.white; // Outline color
    public float outlineThickness = 0.05f;   // Distance of outline from the sprite

    private SpriteRenderer originalSprite;
    private GameObject[] outlineObjects = new GameObject[4];

    void Start()
    {
        originalSprite = GetComponent<SpriteRenderer>();

        if (originalSprite == null)
        {
            Debug.LogError("No SpriteRenderer found on this object!");
            return;
        }

        CreateOutline();
    }

    void CreateOutline()
    {
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(outlineThickness, 0, 0),  // Right
            new Vector3(-outlineThickness, 0, 0), // Left
            new Vector3(0, outlineThickness, 0),  // Up
            new Vector3(0, -outlineThickness, 0)  // Down
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject outline = new GameObject("Outline");
            outline.transform.SetParent(transform);
            outline.transform.localPosition = offsets[i];
            outline.transform.localScale = Vector3.one;

            SpriteRenderer sr = outline.AddComponent<SpriteRenderer>();
            sr.sprite = originalSprite.sprite; 
            sr.sortingLayerID = originalSprite.sortingLayerID;
            sr.sortingOrder = originalSprite.sortingOrder - 1; 

            Material whiteMaterial = new Material(Shader.Find("Custom/UnlitWhiteShader"));
sr.material = whiteMaterial; 

            outlineObjects[i] = outline;
            
        }
    }

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (outlineObjects[i] != null)
            {
                outlineObjects[i].GetComponent<SpriteRenderer>().sprite = originalSprite.sprite;
            }
        }
    }

}
