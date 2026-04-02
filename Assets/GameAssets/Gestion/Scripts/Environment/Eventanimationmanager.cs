using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MapAnimationManager : MonoBehaviour
{
    public static MapAnimationManager Instance { get; private set; }

    [Header("Settings")]
    public E_EventSettings eventSettings;
    public AnimationClip defaultAnimation;

    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private int _lastEventID = -999;

    // On stocke le nom du clip d'origine pour l'override
    private string _baseClipName;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        
        _animator = GetComponent<Animator>();

        // Initialisation de l'Override Controller
        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;

        // On récupère le nom du premier clip utilisé dans l'Animator Controller original
        if (_overrideController.animationClips.Length > 0)
        {
            _baseClipName = _overrideController.animationClips[0].name;
        }
    }

    private void Update()
    {
        int currentID = E_Event.activeEventID;

        // On ne fait rien si l'ID n'a pas changé
        if (currentID == _lastEventID) return;
        
        UpdateMapAnimation(currentID);
        _lastEventID = currentID;
    }

    private void UpdateMapAnimation(int eventID)
    {
        // 1. Si aucun événement (-1), on remet l'animation par défaut
        if (eventID == -1)
        {
            ApplyAnimation(defaultAnimation);
            return;
        }

        // 2. Recherche dans les invasions
        foreach (var invasion in eventSettings.invasionTypes)
        {
            if (invasion.eventID == eventID)
            {
                ApplyAnimation(invasion.mapAnimation);
                return;
            }
        }

        // 3. Recherche dans les événements normaux
        foreach (var normal in eventSettings.normalEvents)
        {
            if (normal.eventID == eventID)
            {
                ApplyAnimation(normal.mapAnimation);
                return;
            }
        }

        // 4. Si ID inconnu, retour au défaut
        ApplyAnimation(defaultAnimation);
    }

    private void ApplyAnimation(AnimationClip clip)
    {
        if (clip == null) clip = defaultAnimation;
        if (clip == null || string.IsNullOrEmpty(_baseClipName)) return;

        // Mise à jour de l'override
        _overrideController[_baseClipName] = clip;

        // Forcer le redémarrage de l'animation pour qu'elle s'actualise immédiatement
        _animator.Play(_baseClipName, 0, 0f);
    }

    /// <summary>
    /// Optionnel : Permet de stopper l'animation manuellement
    /// </summary>
    public void StopAnimation()
    {
        _animator.speed = 0;
    }

    public void ResumeAnimation()
    {
        _animator.speed = 1;
    }
}