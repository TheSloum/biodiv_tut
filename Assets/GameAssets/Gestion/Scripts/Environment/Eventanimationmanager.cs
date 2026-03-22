using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MapAnimationManager : MonoBehaviour
{
    public static MapAnimationManager Instance { get; private set; }

    [Tooltip("Le ScriptableObject EventSettings")]
    public E_EventSettings eventSettings;

    [Tooltip("Glisse ici l'animation par défaut (quand aucun événement)")]
    public AnimationClip defaultAnimation;

    private Animator _animator;
    private AnimatorOverrideController _overrideController;
    private int _lastEventID = -999;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        _animator = GetComponent<Animator>();

        // Crée un Override Controller basé sur le Controller existant
        _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        _animator.runtimeAnimatorController = _overrideController;
    }

    private void Update()
    {
        int currentID = E_Event.activeEventID;

        if (currentID == _lastEventID) return;
        _lastEventID = currentID;

        if (currentID == -1)
        {
            Play(defaultAnimation);
            return;
        }

        foreach (var invasion in eventSettings.invasionTypes)
        {
            if (invasion.eventID == currentID)
            {
                Play(invasion.mapAnimation);
                return;
            }
        }

        foreach (var normal in eventSettings.normalEvents)
        {
            if (normal.eventID == currentID)
            {
                Play(normal.mapAnimation);
                return;
            }
        }

        Play(defaultAnimation);
    }

    private void Play(AnimationClip clip)
    {
        if (clip == null) { Debug.LogWarning("[MapAnimationManager] Clip null !"); return; }

        // Récupère le clip actuellement dans le controller et le remplace
        var clips = _overrideController.animationClips;
        if (clips.Length == 0) { Debug.LogWarning("[MapAnimationManager] Aucun clip dans le Controller !"); return; }

        // Remplace le premier clip (le seul état de ta map) par le nouveau
        _overrideController[clips[0].name] = clip;

        Debug.Log($"[MapAnimationManager] Clip remplacé : '{clips[0].name}' → '{clip.name}'");
    }
}