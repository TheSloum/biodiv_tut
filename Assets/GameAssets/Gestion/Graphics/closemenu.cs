using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class closemenu : MonoBehaviour
{
    public GameObject close;

    public void closer()
    {
        close.SetActive(false);
    }
}
