using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlockPages : MonoBehaviour
{
    [SerializeField] private GameObject page1;
    [SerializeField] private GameObject page2;
    [SerializeField] private GameObject page3;
    [SerializeField] private GameObject page4;
    [SerializeField] private GameObject page5;

    [SerializeField] private Button[] buttons;

    [SerializeField] private Sprite[] activeSprites;
    [SerializeField] private Sprite[] inactiveSprites;

    private int currentPage = 1;

    public AudioClip sfxClip;
    void Update()
    {
        UpdateButtonSprites();
    }

    public void PageChange(int page)
    {
        // Désactive toutes les pages
        page1.SetActive(false);
        page2.SetActive(false);
        page3.SetActive(false);
        page4.SetActive(false);
        page5.SetActive(false);

        // Active uniquement la page sélectionnée
        if (page == 1) page1.SetActive(true);
        else if (page == 2) page2.SetActive(true);
        else if (page == 3) page3.SetActive(true);
        else if (page == 4) page4.SetActive(true);
        else if (page == 5) page5.SetActive(true);

        currentPage = page; // Met à jour la page actuelle
        UpdateButtonSprites(); // Met à jour les sprites des boutons
    }

    private void UpdateButtonSprites()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == currentPage - 1) // Si c'est le bouton correspondant à la page actuelle
            {
                buttons[i].image.sprite = activeSprites[i]; // Met le sprite actif
            }
            else
            {
                buttons[i].image.sprite = inactiveSprites[i]; // Met le sprite inactif
            }
        }
    }

    public void ClosePage()
    {
        SoundManager.instance.PlaySFX(sfxClip);
        gameObject.SetActive(false);
        Materials.instance.canMove = true;
    }
}
