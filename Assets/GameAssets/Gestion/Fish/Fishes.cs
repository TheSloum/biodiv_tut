using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFish", menuName = "Fish")]
public class Fishes : ScriptableObject
{
    public int fishID = 0;
    public string fishName = "Merlu";
    public bool is_unlocked = false; // Défini sur false par défaut
    public string fishDes = "Ce poisson a une grosse tête de con et il nage. \n En plus, il est pas bon.";
    public string fishOrigin = "Martinique du far-ouest";
    public string fishSize = "25m";

    public enum fishClasstypes
    {
        Vertébré,
        Invertébré,
        Miamousse
    }
    public fishClasstypes fishClass;
    public Sprite fishSprite;

    /// <summary>
    /// Déverrouille le poisson.
    /// </summary>
    public void UnlockFish()
    {
        if (!is_unlocked)
        {
            is_unlocked = true;
            // Ajoute ici des actions supplémentaires, comme l'affichage d'une notification
            Debug.Log($"Poisson '{fishName}' avec ID {fishID} a été déverrouillé !");
        }
    }
}
