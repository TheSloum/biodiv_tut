using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryPage : MonoBehaviour
{
    public GameObject infoCardPrefab;
    public List<Fishes> fishesList;
    public GameObject containerToMove;

    public Button btnLeft;
    public Button btnRight;

    private TMP_Text fishNameText;
    private TMP_Text fishSizeText;
    private TMP_Text fishClassText;
    private TMP_Text fishOriginText;
    private TMP_Text fishDescriptionText;
    private TMP_Text frequenceText;
    private TMP_Text mediumWeightText;
    private TMP_Text profondeurText;
    private TMP_Text eatText;
    private TMP_Text lifeTimeText;
    private TMP_Text fishScientistNameText;
    private Image fishImage;
    private Image fishPixelImage;

    private int currentIndex = 0;
    private int maxIndex = 3;
    private float moveDistance = 800f;
    private float animationDuration = 0.2f;
    private bool isMoving = false; // Empêche plusieurs animations en même temps

    public void Start()
    {
        fishNameText = FindDeepChild(infoCardPrefab.transform, "fishNameText").GetComponent<TMP_Text>();
        fishSizeText = FindDeepChild(infoCardPrefab.transform, "fishSizeText").GetComponent<TMP_Text>();
        fishClassText = FindDeepChild(infoCardPrefab.transform, "fishClassText").GetComponent<TMP_Text>();
        fishOriginText = FindDeepChild(infoCardPrefab.transform, "fishOriginText").GetComponent<TMP_Text>();
        fishDescriptionText = FindDeepChild(infoCardPrefab.transform, "fishDescriptionText").GetComponent<TMP_Text>();
        fishImage = FindDeepChild(infoCardPrefab.transform, "fishImage").GetComponent<Image>();
        fishPixelImage = FindDeepChild(infoCardPrefab.transform, "fishPixelImage").GetComponent<Image>();
        frequenceText = FindDeepChild(infoCardPrefab.transform, "frequenceText").GetComponent<TMP_Text>();
        mediumWeightText = FindDeepChild(infoCardPrefab.transform, "mediumWeightText").GetComponent<TMP_Text>();
        profondeurText = FindDeepChild(infoCardPrefab.transform, "profondeurText").GetComponent<TMP_Text>();
        eatText = FindDeepChild(infoCardPrefab.transform, "eatText").GetComponent<TMP_Text>();
        lifeTimeText = FindDeepChild(infoCardPrefab.transform, "lifeTimeText").GetComponent<TMP_Text>();
        fishScientistNameText = FindDeepChild(infoCardPrefab.transform, "fishScientistNameText").GetComponent<TMP_Text>();

        infoCardPrefab.SetActive(false);
        UpdateButtons();
    }

    public static Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            Transform found = FindDeepChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }

    public void ShowFishInfo(int fishID)
    {
        Fishes fish = fishesList.Find(f => f.fishID == fishID);

        if (fish != null && fish.is_unlocked)
        {
            UpdateTextComponent(fishNameText, fish.fishName);
            UpdateTextComponent(fishSizeText, fish.fishSize, "Taille : ");
            UpdateTextComponent(fishClassText, fish.fishClass, "Classification : ");
            UpdateTextComponent(fishOriginText, fish.fishOrigin, "Habitat : ");
            UpdateTextComponent(fishDescriptionText, fish.fishDes);
            UpdateTextComponent(frequenceText, fish.frequence, "Fréquence : ");
            UpdateTextComponent(mediumWeightText, fish.mediumWeight, "Poids : ");
            UpdateTextComponent(profondeurText, fish.profondeur, "Profondeur : ");
            UpdateTextComponent(eatText, fish.eat, "Régime : ");
            UpdateTextComponent(lifeTimeText, fish.lifeTime, "Espérance de vie : ");
            UpdateTextComponent(fishScientistNameText, fish.fishScientistName, "Nom scientifique : ");

            fishImage.sprite = fish.fishSprite;
            fishImage.gameObject.SetActive(fish.fishSprite != null);

            fishPixelImage.sprite = fish.fishSpritePixel;
            fishPixelImage.gameObject.SetActive(fish.fishSpritePixel != null);

            infoCardPrefab.SetActive(true);
        }
    }

    private void UpdateTextComponent(TMP_Text textComponent, string value, string prefix = "")
    {
        textComponent.text = string.IsNullOrEmpty(value) ? "" : prefix + value;
        textComponent.gameObject.SetActive(!string.IsNullOrEmpty(value));
    }

    public void HideFishInfo()
    {
        infoCardPrefab.SetActive(false);
    }

    public void GalleryClose()
    {
        gameObject.SetActive(false);
    }

    public void MoveLeft()
    {
        if (currentIndex < maxIndex && !isMoving)
        {
            currentIndex++;
            float newXPosition = containerToMove.transform.localPosition.x - moveDistance;
            StartCoroutine(AnimateMove(newXPosition));
        }
    }

    public void MoveRight()
    {
        if (currentIndex > 0 && !isMoving)
        {
            currentIndex--;
            float newXPosition = containerToMove.transform.localPosition.x + moveDistance;
            StartCoroutine(AnimateMove(newXPosition));
        }
    }

    private IEnumerator AnimateMove(float targetX)
    {
        isMoving = true;
        float elapsedTime = 0;
        Vector3 startPosition = containerToMove.transform.localPosition;
        Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            containerToMove.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        containerToMove.transform.localPosition = targetPosition;
        isMoving = false;

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        btnLeft.gameObject.SetActive(currentIndex < maxIndex);
        btnRight.gameObject.SetActive(currentIndex > 0);
    }
}
