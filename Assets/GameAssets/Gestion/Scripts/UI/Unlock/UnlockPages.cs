using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockPages : MonoBehaviour
{

    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;
    [SerializeField] private GameObject page3;
    [SerializeField] private GameObject page4;
    [SerializeField] private GameObject page5;

    public AudioClip sfxClip;
    public AudioClip musicClip;

    public void PageChange(int page)
    {
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);

        if (page == 1)
        {
            page1.SetActive(true);
        }
        else if (page == 2)
        {
            page2.SetActive(true);
        }
        else if (page == 3)
        {
            page3.SetActive(true);
        }
        else if (page == 4)
        {
            page4.SetActive(true);
        }
        else if (page == 5)
        {
            page5.SetActive(true);
        }
    }
    public void ClosePage()
    {
        SoundManager.instance.PlayMusic(musicClip);
        SoundManager.instance.PlaySFX(sfxClip);
        gameObject.SetActive(false);
    }
}
