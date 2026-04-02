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
        if (Materials.instance == null) return;

        if (textFields.Length >= 1) textFields[0].text = Materials.instance.mat_0.ToString();
        if (textFields.Length >= 2) textFields[1].text = Materials.instance.mat_1.ToString();
        if (textFields.Length >= 3) textFields[2].text = Materials.instance.mat_2.ToString();
        if (textFields.Length >= 4) textFields[3].text = Materials.instance.price.ToString();

        if (textFields.Length >= 5 && textFields[4] != null)
        {
            textFields[4].text = Materials.instance.trash.ToString();
        }
    }
}
