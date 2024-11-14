using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishUnlock : MonoBehaviour
{
    public int fishunlockID = 0;

        public List<Fishes> fishes;




    public void unlockFish(){
            foreach (Fishes fish in fishes)
        {
            if (fish.fishID == fishunlockID)
            {

                fish.UnlockFish();
                return;
            }
        }
        
    }
}
