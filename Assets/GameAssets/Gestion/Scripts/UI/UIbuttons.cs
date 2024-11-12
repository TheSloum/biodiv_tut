using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbuttons : MonoBehaviour
{
    [SerializeField] private GameObject unlockMenu ;
    void Start()
    {
        
    }

public void UnlockMenuOpen(){
        unlockMenu.SetActive(true);
}
}
