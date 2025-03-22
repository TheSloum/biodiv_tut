using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManagement : MonoBehaviour
{
    public AudioClip sfxClip;
    void Start()
    {

    }

    public void ClosePage(GameObject Page)
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Page.SetActive(false);
    }
    public void OpenPage(GameObject Page)
    {
        SoundManager.instance.PlaySFX(sfxClip);
        Page.SetActive(true);
    }
}
