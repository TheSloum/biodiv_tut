using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unlock : MonoBehaviour
{
    public List<Building> buildings;
    public Sprite baseSprite; // Sprite pour les boutons déverrouillés
    public Sprite selectedSprite;
    public Sprite lockedSprite; // Sprite pour les boutons verrouillés
    public Sprite confirmNormalSprite; // Sprite pour le bouton de confirmation lorsque l'utilisateur a assez d'argent
    public Sprite confirmInsufficientSprite; // Sprite pour le bouton de confirmation lorsque l'utilisateur n'a pas assez d'argent

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
    private Button tempButton; // Stocke le bouton en attente de confirmation
    private Button lastSelectedButton; // Le dernier bouton sélectionné

    void Start()
    {
        confirmButton.SetActive(false);

        // Initialiser l'état des boutons au démarrage
        foreach (var pair in buttonBuildingPairs)
        {
            if (pair.building.unlocked)
            {
                // Si le bâtiment est déjà déverrouillé, appliquer baseSprite et désactiver l'interaction
                SetButtonSprite(pair.button, baseSprite, selectedSprite);
                pair.button.interactable = false; // Désactive l'interaction (survol et clic)
            }
            else
            {
                // Si le bâtiment est verrouillé, appliquer lockedSprite et rendre interactif
                SetButtonSprite(pair.button, lockedSprite, selectedSprite);
                pair.button.interactable = true; // L'option reste interactive
            }
        }
    }

    void Update()
    {
        // Vérifie si l'utilisateur a suffisamment d'argent en temps réel et met à jour le bouton de confirmation
        if (confirmButton.activeSelf) // S'assurer que le bouton de confirmation est actif
        {
            if (Materials.instance.price >= tempPrice)
            {
                // L'utilisateur a assez d'argent, appliquer le sprite normal
                SetConfirmButtonSprite(confirmNormalSprite);
            }
            else
            {
                // L'utilisateur n'a pas assez d'argent, appliquer le sprite d'insuffisance
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

            // Si un bouton a déjà été sélectionné, on le réinitialise
            if (lastSelectedButton != null && lastSelectedButton != tempButton)
            {
                // Réinitialiser le dernier bouton sélectionné à son état d'origine
                SetButtonSprite(lastSelectedButton, lockedSprite, selectedSprite);
                lastSelectedButton.interactable = true; // Rendre le dernier bouton interactif à nouveau
            }

            // Appliquer le sprite "sélectionné" au nouveau bouton cliqué
            SetButtonSprite(tempButton, selectedSprite, selectedSprite);
            tempButton.interactable = false; // Désactiver l'interaction sur ce bouton après sélection

            // Mémoriser le dernier bouton sélectionné
            lastSelectedButton = tempButton;
        }

        confirmButton.SetActive(true);
    }

    public void ConfirmUnlock()
    {
        // Vérifier si l'utilisateur a assez d'argent
        if (Materials.instance.price >= tempPrice)
        {
            foreach (Building building in buildings)
            {
                if (building.buildID == tempUnlockID && !building.unlocked)
                {
                    building.Unlock();
                    Materials.instance.price -= tempPrice;

                    // Après confirmation, appliquer baseSprite (déverrouiller) et désactiver l'interaction
                    if (tempButton != null)
                    {
                        SetButtonSprite(tempButton, baseSprite, selectedSprite);
                        tempButton.interactable = false; // Désactiver l'interaction après le déverrouillage
                    }
                }
            }

            // Mettre à jour le sprite de confirmation (même si l'utilisateur a assez d'argent)
            SetConfirmButtonSprite(confirmNormalSprite);
        }
        else
        {
            // Si l'utilisateur n'a pas assez d'argent, appliquer le sprite pour l'état non suffisant
            SetConfirmButtonSprite(confirmInsufficientSprite);
        }

        confirmButton.SetActive(false);
        tempButton = null; // Réinitialisation
        lastSelectedButton = null; // Réinitialisation du bouton sélectionné
    }

    public void CancelUnlock()
    {
        // Réinitialiser le dernier bouton sélectionné s'il y en avait un
        if (lastSelectedButton != null)
        {
            SetButtonSprite(lastSelectedButton, lockedSprite, selectedSprite); // Réinitialiser à lockedSprite
            lastSelectedButton.interactable = true; // Rendre le bouton interactif à nouveau
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
    }

    private void SetConfirmButtonSprite(Sprite sprite)
    {
        Image confirmButtonImage = confirmButton.GetComponent<Image>();
        confirmButtonImage.sprite = sprite;
    }
}
