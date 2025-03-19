using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class J_FullReset : MonoBehaviour
{
    public List<Fishes> fishes;

    
    public List<Building> buildings;
    void Awake() 
    {
        
        foreach (Building building in buildings)
{
    building.Relock();
    Debug.Log("ZRQEPOKJRPOJZORFJQFPOJPOEJPOTEGSJ");
}

foreach (Fishes fish in fishes)
{
    fish.RelockFish();
    Debug.Log("ZRQEPOKJRPOJZORFJQFPOJPOEJPOTEGSJ");
}
    }

}
