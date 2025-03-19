using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_PolutionGestion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer PoluImg;
    [SerializeField] private List<Sprite> PolustepImg;

    void Awake()
    {
        if (Materials.instance != null)
        {
            if (Materials.instance.bar_2 <= 0.25f)
            {
                PoluImg.sprite = PolustepImg[0];
            }
            else if (Materials.instance.bar_2 <= 0.5f)
            {
                PoluImg.sprite = PolustepImg[1];
            }
            else if (Materials.instance.bar_2 <= 0.75f)
            {
                PoluImg.sprite = PolustepImg[2];
            }
            else
            {
                PoluImg.sprite = PolustepImg[3];
            }
        }
    }
}
