using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewEventDefinition", menuName = "Events/New Event Definition")]
public class E_EventDefinition : ScriptableObject
{
    [Header("Informations sur l'événement")]
    public int eventID;
    public string eventName;
    [TextArea]
    public string message;
    public Color messageColor = Color.white;

    [Header("Durées (en secondes)")]
    public float textFadeInDuration = 1f;
    public float textVisibleDuration = 2f;
    public float textFadeOutDuration = 1f;
    public float eventDuration = 10f;

    [Header("Callbacks")]
    public UnityEvent OnEventStart;
    public UnityEvent OnEventEnd;
}
