using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fish")]
public class Fishes : ScriptableObject
{
    public int fishID = 0;
    public string fishName = "Salut";
    public string fishScientistName = "Hippocampus hippocampus";
    public string fishSize = "25m";
    public string fishOrigin = " herbiers, algues de zones rocheuses";
    public string frequence = "rare";
    public float freqWeight = 1f;
    public string fishClass = "Syngnathidae";
    public string mediumWeight = "0.015kg";
    public string profondeur = "10-30m";

    public string eat = "Planctophage";
    public string lifeTime = "2-4 ans";

    public bool is_unlocked = false; // Défini sur false par défaut
    public string fishDes = "Ce poisson a une grosse tête de con et il nage. \n En plus, il est pas bon.";
    public Sprite fishSprite;
    public Sprite fishSpritePixel;

    /// <summary>
    /// Déverrouille le poisson.
    /// </summary>
    public void UnlockFish()
    {
            is_unlocked = true;
            // Ajoute ici des actions supplémentaires, comme l'affichage d'une notification
            Debug.Log($"Poisson '{fishName}' avec ID {fishID} a été déverrouillé !");
        
    }
    public void RelockFish()
    {
            is_unlocked = false;
        
    }
}
