using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : MonoBehaviour
{


        public List<Building> buildings;


    void Start(){
        
    }
    public void unlockBuild(string priceAndID){
        string[] parts = priceAndID.Split(',');
        int price = int.Parse(parts[0]);
        int unlockID = int.Parse(parts[1]);


        if(Materials.instance.price >= price){
            foreach (Building building in buildings)
        {
            if (building.buildID == unlockID && !building.unlocked)
            {

                building.Unlock();
                Materials.instance.price -= price;
                return;
            }
        }
        }
    }
}
