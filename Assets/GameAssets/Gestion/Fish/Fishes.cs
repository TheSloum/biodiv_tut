using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Fishes : ScriptableObject
{
    public int fishID = 0;

    
    public string fishName = "Merlu";
    public bool is_unlocked = true;

    public void UnlockFish()
    {
        unlocked = true;
    }
    public string fishDes = "Ce poisson a une grosse tête de con et il nage. \n En plus, il est pas bon.";
    public string fishOrigin = "Martinique du far-ouest";
    public string fishSize = "25m";

    public enum fishClasstypes
{
    vertébré,
    invertébré,
    miamousse
}
    public fishClasstypes fishClass;
    public Sprite fishSprite;
}