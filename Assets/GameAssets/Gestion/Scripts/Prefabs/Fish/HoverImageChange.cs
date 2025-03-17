using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HoverImageChange : MonoBehaviour
{
    private Image parentImage;
    private Image childImage;
    private GameObject fishImg;
    private GameObject textObj;
    private TextMeshProUGUI nameSpriteText;
    private TextMeshProUGUI questionMarkText;

    public Sprite parentHoverSprite;
    public Sprite childHoverSprite;
    private Sprite parentOriginalSprite;
    private Sprite childOriginalSprite;

    public Fishes fish;

    void Start()
    {
        parentImage = GetComponent<Image>();
        childImage = transform.Find("NameSprite")?.GetComponent<Image>();

        fishImg = transform.Find("FishImg")?.gameObject;
        textObj = transform.Find("Text (TMP)")?.gameObject;

        nameSpriteText = transform.Find("NameSprite/Text (TMP)")?.GetComponent<TextMeshProUGUI>();
        questionMarkText = transform.Find("NameSprite/?")?.GetComponent<TextMeshProUGUI>();

        if (parentImage != null)
            parentOriginalSprite = parentImage.sprite;

        if (childImage != null)
            childOriginalSprite = childImage.sprite;

        EventTrigger trigger = gameObject.GetComponent<EventTrigger>() ?? gameObject.AddComponent<EventTrigger>();

        if (fish != null && fish.is_unlocked)
        {
            AddEvent(trigger, EventTriggerType.PointerEnter, OnParentHoverEnter);
            AddEvent(trigger, EventTriggerType.PointerExit, OnParentHoverExit);
            AddEvent(trigger, EventTriggerType.PointerEnter, OnChildHoverEnter);
            AddEvent(trigger, EventTriggerType.PointerExit, OnChildHoverExit);
        }

        if (fish != null && fish.is_unlocked == false)
        {
            if (fishImg != null) fishImg.SetActive(false);
            if (textObj != null) textObj.SetActive(true);
        }
        else
        {
            if (fishImg != null) fishImg.SetActive(true);
            if (textObj != null) textObj.SetActive(false);
        }

        if (nameSpriteText != null && questionMarkText != null)
        {
            if (fish != null && fish.is_unlocked == false)
            {
                nameSpriteText.gameObject.SetActive(false);
                questionMarkText.gameObject.SetActive(true);
            }
            else
            {
                nameSpriteText.gameObject.SetActive(true);
                questionMarkText.gameObject.SetActive(false);
            }
        }
    }

    void AddEvent(EventTrigger trigger, EventTriggerType eventType, System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action());
        trigger.triggers.Add(entry);
    }

    public void OnParentHoverEnter()
    {
        if (fish != null && fish.is_unlocked)
        {
            if (childImage != null && childHoverSprite != null)
                childImage.sprite = childHoverSprite;

            if (parentImage != null && parentHoverSprite != null)
                parentImage.sprite = parentHoverSprite;
        }
    }

    public void OnParentHoverExit()
    {
        if (fish != null && fish.is_unlocked)
        {
            if (childImage != null)
                childImage.sprite = childOriginalSprite;

            if (parentImage != null)
                parentImage.sprite = parentOriginalSprite;
        }
    }

    public void OnChildHoverEnter()
    {
        if (fish != null && fish.is_unlocked)
        {
            if (childImage != null && childHoverSprite != null)
                childImage.sprite = childHoverSprite;

            if (parentImage != null && parentHoverSprite != null)
                parentImage.sprite = parentHoverSprite;
        }
    }

    public void OnChildHoverExit()
    {
        if (fish != null && fish.is_unlocked)
        {
            if (childImage != null)
                childImage.sprite = childOriginalSprite;

            if (parentImage != null)
                parentImage.sprite = parentOriginalSprite;
        }
    }
}
