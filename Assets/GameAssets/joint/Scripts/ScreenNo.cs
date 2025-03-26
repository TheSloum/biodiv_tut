using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ScreenNo : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {

        GameObject loadingObject = Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(g => g.name == "loadingScreen");

        loadingObject.SetActive(false);
    }

}
