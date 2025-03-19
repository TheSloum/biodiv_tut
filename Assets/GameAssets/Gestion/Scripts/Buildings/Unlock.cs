using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    public List<Building> buildings;
    public Sprite baseSprite;
    public Sprite selectedSprite;
    public Sprite lockedSprite;
    public Sprite confirmNormalSprite;
    public Sprite confirmInsufficientSprite;

    [System.Serializable]
    public class ButtonBuildingPair
    {
        public Button button;
        public Building building;
    }

    public List<ButtonBuildingPair> buttonBuildingPairs;

    [Header("Confirmation UI")]
    public GameObject confirmButton;

    private int tempPrice;
    private int tempUnlockID;
    private Button tempButton;
    private Button lastSelectedButton;

    void Start()
    {
        confirmButton.SetActive(false);

        foreach (var pair in buttonBuildingPairs)
        {
            if (pair.building.unlocked)
            {
                SetButtonSprite(pair.button, baseSprite, selectedSprite);
                pair.button.interactable = false;
            }
            else
            {
                SetButtonSprite(pair.button, lockedSprite, selectedSprite);
                pair.button.interactable = true;
            }
        }
    }

    void Update()
    {

        if (confirmButton.activeSelf)
        {
            if (Materials.instance.price >= tempPrice)
            {

                SetConfirmButtonSprite(confirmNormalSprite);
            }
            else
            {
                SetConfirmButtonSprite(confirmInsufficientSprite);
            }
        }
    }

    public void PrepareUnlock(string priceAndID)
    {
        string[] parts = priceAndID.Split(',');
        tempPrice = int.Parse(parts[0]);
        tempUnlockID = int.Parse(parts[1]);

        string buttonName = "UnlockBuild_" + tempUnlockID.ToString();
        GameObject buttonObject = GameObject.Find(buttonName);


        if (buttonObject != null)
        {
            tempButton = buttonObject.GetComponent<Button>();

            if (lastSelectedButton != null && lastSelectedButton != tempButton)
            {
                SetButtonSprite(lastSelectedButton, lockedSprite, selectedSprite);
                lastSelectedButton.interactable = true;
            }

            SetButtonSprite(tempButton, selectedSprite, selectedSprite);
            tempButton.interactable = false;

            lastSelectedButton = tempButton;
        }

        confirmButton.SetActive(true);
    }

    public void ConfirmUnlock()
    {

        if (Materials.instance.price >= tempPrice)
        {
            foreach (Building building in buildings)
            {
                if (building.buildID == tempUnlockID && !building.unlocked)
                {
                    building.Unlock();
                    Materials.instance.price -= tempPrice;

                    if (tempButton != null)
                    {
                        SetButtonSprite(tempButton, baseSprite, selectedSprite);
                        tempButton.interactable = false;
                    }
                }
            }

            SetConfirmButtonSprite(confirmNormalSprite);
        }
        else
        {
            SetConfirmButtonSprite(confirmInsufficientSprite);
        }

        confirmButton.SetActive(false);
        tempButton = null;
        lastSelectedButton = null;
    }

    public void CancelUnlock()
    {
        if (lastSelectedButton != null)
        {
            SetButtonSprite(lastSelectedButton, lockedSprite, selectedSprite);
            lastSelectedButton.interactable = true;
        }

        confirmButton.SetActive(false);
        tempButton = null;
        lastSelectedButton = null;
    }

    private void SetButtonSprite(Button button, Sprite normal, Sprite selected)
    {
        Image buttonImage = button.GetComponent<Image>();
        buttonImage.sprite = normal;
        SpriteState spriteState = button.spriteState;
        spriteState.selectedSprite = selected;
        button.spriteState = spriteState;

        if (normal == baseSprite)
        {
            Transform priceTransform = button.transform.Find("Price");
            if (priceTransform != null)
            {
                priceTransform.gameObject.SetActive(false);
            }
        }
        else
        {
            Transform priceTransform = button.transform.Find("Price");
            if (priceTransform != null)
            {
                priceTransform.gameObject.SetActive(true);
            }
        }
    }


    private void SetConfirmButtonSprite(Sprite sprite)
    {
        Image confirmButtonImage = confirmButton.GetComponent<Image>();
        confirmButtonImage.sprite = sprite;
    }
}
