using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlock : MonoBehaviour
{
    [SerializeField] private GameObject player; 

    private Materials playerMat;


    public int unlockID = 0;
    public int price = 0;

        public List<Building> buildings;


    void Start(){
        
        playerMat = player.GetComponent<Materials>();
    }
    public void unlockBuild(){
        if(playerMat.price > price){
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
