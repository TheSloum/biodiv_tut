using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dechetMap : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; 
    public Sprite spritePlein; 
    public Sprite spriteMoyenPlus;
    public Sprite spriteMoyenMoins;
    public Sprite spriteVide;  
    void Start()
    {

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        float val = Materials.instance.bar_2;

        if (val >= 0.75f)
        {
            spriteRenderer.sprite = spritePlein;
        }   
        else if (val < 0.75f && val >= 0.50f)
        {
            spriteRenderer.sprite = spriteMoyenPlus;
        }    
        else if (val < 0.50f && val >= 0.25f)
        {
            spriteRenderer.sprite = spriteMoyenMoins;
        }    
        else 
        {
            spriteRenderer.sprite = spriteVide;
        }    
    }
}