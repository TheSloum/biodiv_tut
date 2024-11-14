using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryPage : MonoBehaviour
{
    public GameObject infoCardPrefab; 
    public List<Fishes> fishesList;  

    private TMP_Text fishNameText;
    private TMP_Text fishSizeText;
    private TMP_Text fishClassText;
    private TMP_Text fishOriginText;
    private TMP_Text fishDescriptionText;
    private SpriteRenderer fishImage;

    public void Start(){
         fishNameText = infoCardPrefab.transform.Find("FishName").GetComponent<TMP_Text>();
        fishSizeText = infoCardPrefab.transform.Find("FishSize").GetComponent<TMP_Text>();
        fishClassText = infoCardPrefab.transform.Find("FishClass").GetComponent<TMP_Text>();
        fishOriginText = infoCardPrefab.transform.Find("FishOrig").GetComponent<TMP_Text>();
        fishDescriptionText = infoCardPrefab.transform.Find("FishDesc").GetComponent<TMP_Text>();
        fishImage = infoCardPrefab.transform.Find("FishImg").GetComponent<SpriteRenderer>();

        infoCardPrefab.SetActive(false);
    }

    public void ShowFishInfo(int fishID)
    {
        Fishes fish = fishesList.Find(f => f.fishID == fishID);

        if (fish != null && fish.is_unlocked) 
        {
            fishNameText.text = fish.fishName;
            fishSizeText.text = fish.fishSize;
            fishClassText.text = fish.fishClass.ToString();
            fishOriginText.text = fish.fishOrigin;
            fishDescriptionText.text = fish.fishDes;
            fishImage.sprite = fish.fishSprite;

            infoCardPrefab.SetActive(true);
        }
    }
    public void HideFishInfo()
    {
        infoCardPrefab.SetActive(false);
    }
public void GalleryClose(){
        gameObject.SetActive(false);
}

public void GalleryOpen(){
        gameObject.SetActive(true);
}
public void ShowFish(){

}
}