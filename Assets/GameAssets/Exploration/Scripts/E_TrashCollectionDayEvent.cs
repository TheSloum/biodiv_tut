using UnityEngine;

[CreateAssetMenu(fileName = "TrashCollectionDay", menuName = "ScriptableObjects/Events/TrashCollectionDay")]
public class E_TrashCollectionDayEvent : ScriptableObject
{
    // Configuration de l'événement
    [Header("Paramètres de l'événement")]
    public int eventID = 10; // Assurez-vous que cet ID est unique et n'est pas utilisé par d'autres événements
    public string eventName = "Journée ramassage des déchets";
    
    [TextArea(3, 5)]
    public string message = "Une journée de ramassage des déchets est organisée ! Les habitants se mobilisent pour nettoyer l'environnement.";
    public Color messageColor = Color.green;
    
    [Header("Effets sur la gestion")]
    [Tooltip("Augmentation de la qualité de vie")]
    public float qualityOfLifeBoost = 0.1f;
    [Tooltip("Réduction importante de la pollution")]
    public float pollutionReduction = -0.2f;
    [Tooltip("Coût de l'événement")]
    public int moneyCost = -200;
    
    [Header("Effets sur l'exploration")]
    [Tooltip("Réduction du taux de spawn des déchets pendant l'événement")]
    public float trashSpawnRateMultiplier = 0.5f;
    [Tooltip("Bonus pour les poissons attrapés pendant l'événement")]
    public float fishValueBonus = 1.2f;
}