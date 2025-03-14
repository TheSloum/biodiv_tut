using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbuttons : MonoBehaviour
{
    public AudioClip sfxClip;
    public AudioClip musicClip;
    [SerializeField] private GameObject unlockMenu;
    void Start()
    {
    }

    public void UnlockMenuOpen()
    {
        unlockMenu.SetActive(true);
        Materials.instance.canMove = false;

    }
}
