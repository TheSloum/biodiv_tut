using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Unlock : MonoBehaviour
{


        public List<Building> buildings;
        public Sprite baseSprite; 
    public Sprite selectedSprite;

    void Start(){
        
    }
    public void unlockBuild(string priceAndID){
        Debug.Log(priceAndID);
        string[] parts = priceAndID.Split(',');
        int price = int.Parse(parts[0]);
        int unlockID = int.Parse(parts[1]);



        if(Materials.instance.price >= price){
            foreach (Building building in buildings)
        {
            if (building.buildID == unlockID && !building.unlocked)
            {

                
        string buttonName = "UnlockBuild_" + unlockID.ToString();

        GameObject buttonObject = GameObject.Find(buttonName);
        Button button = buttonObject.GetComponent<Button>();
                Image buttonImage = button.GetComponent<Image>();
        buttonImage.sprite = baseSprite;
        SpriteState spriteState = button.spriteState;
        spriteState.selectedSprite = selectedSprite;
        button.spriteState = spriteState;

                building.Unlock();
                
        Debug.Log(building.unlocked);
                Materials.instance.price -= price;
                return;
            }
        }
        }
    }
}
