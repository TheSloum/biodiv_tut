using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayMats : MonoBehaviour
{
    public TextMeshProUGUI[] textFields;
    void Start()
    {
        
    }

    void Update()
    {
        if (Materials.instance != null)
        {
            textFields[0].text = Materials.instance.mat_0.ToString();
            textFields[1].text = Materials.instance.mat_1.ToString();
            textFields[2].text = Materials.instance.mat_2.ToString();
            textFields[3].text = Materials.instance.price.ToString();
        }
    }
}
