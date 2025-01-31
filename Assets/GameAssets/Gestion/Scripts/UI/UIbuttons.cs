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
    SoundManager.instance.PlayMusic(musicClip);
    SoundManager.instance.PlaySFX(sfxClip);
    unlockMenu.SetActive(true);
    Materials.instance.canMove = false;

}
}
