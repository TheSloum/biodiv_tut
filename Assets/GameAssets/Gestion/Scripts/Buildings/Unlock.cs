using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : MonoBehaviour
{

    public int unlockID = 0;
    public int price = 0;

        public List<Building> buildings;


    void Start(){
        
    }
    public void unlockBuild(){
        if(Materials.instance.price > price){
            foreach (Building building in buildings)
        {
            if (building.buildID == unlockID)
            {

                building.Unlock();
                return;
            }
        }
        }
    }
}
