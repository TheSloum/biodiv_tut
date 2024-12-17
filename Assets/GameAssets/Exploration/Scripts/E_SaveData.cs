using System;
using UnityEngine;

[Serializable]
public class E_SaveData
{
    public Vector3 playerPosition;
    public float currentOxygen;
    public int trashCollected;

    // Ajout pour le temps
    public int currentDay;
    public int currentMonth;
}
