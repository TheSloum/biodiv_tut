using UnityEngine;
using System.Collections.Generic;

public class BuilderAnimationController : MonoBehaviour
{
    public AnimationClip pizzeriaAnim;
    public AnimationClip thermAnim;
    public AnimationClip hydrauAnim;
    public AnimationClip jardinAnim;
    public AnimationClip build1Anim;
    public AnimationClip build2Anim;

    // Provide a base Animator Controller with a state named "Animation"
    public RuntimeAnimatorController baseAnimatorController;

    // Keep track of each builder's last sprite name
    private Dictionary<Builder, string> builderSpriteMap = new Dictionary<Builder, string>();

    // Mapping from sprite names to their corresponding animation clips
    private Dictionary<string, AnimationClip> spriteToAnim;
    public bool anim = true;

    void Start()
    {
        // Create the sprite-to-animation mapping
        spriteToAnim = new Dictionary<string, AnimationClip>()
        {
            { "PIZZERIA", pizzeriaAnim },
            
            { "centrale thermique", thermAnim },
            { "HYDRAULIQUE_COMPLET", hydrauAnim },
            { "JARDIN", jardinAnim },
            { "construction V1 type 2", build1Anim },
            { "grandeConstruction1", build2Anim }
        };

        // Find all builder objects and initialize their animation state
        Builder[] builders = FindObjectsOfType<Builder>();
        foreach (Builder builder in builders)
        {
            if (builder != null)
            {
                SpriteRenderer sr = builder.GetComponent<SpriteRenderer>();
                if (sr != null && sr.sprite != null)
                {
                    // Store the initial sprite name
                    builderSpriteMap[builder] = sr.sprite.name;

                    // If this sprite has an animation, ensure the Animator component exists and play it
                    if (spriteToAnim.ContainsKey(sr.sprite.name))
                    {
                        EnsureAnimatorComponent(builder.gameObject);
                        PlayAnimation(builder.gameObject, spriteToAnim[sr.sprite.name]);
                    }
                }
            }
        }
    }

    void Update()
    {
        // Check each builder to see if its sprite has changed
        Builder[] builders = FindObjectsOfType<Builder>();
        foreach (Builder builder in builders)
        {
            if (builder != null)
            {
                SpriteRenderer sr = builder.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                string currentSpriteName = (sr.sprite != null) ? sr.sprite.name : "";
                string previousSpriteName;
                builderSpriteMap.TryGetValue(builder, out previousSpriteName);

                // If the sprite changed, update our record and adjust animations
                if (currentSpriteName != previousSpriteName)
                {
                    Debug.Log("Sprite changed: " + previousSpriteName + " -> " + currentSpriteName);
                    builderSpriteMap[builder] = currentSpriteName;
                    
                    // If the new sprite corresponds to an animation, play it
                    if (spriteToAnim.ContainsKey(currentSpriteName) && anim == false)
                    {
                        anim = false;
                        EnsureAnimatorComponent(builder.gameObject);
                        PlayAnimation(builder.gameObject, spriteToAnim[currentSpriteName]);
                    }
                }
            }
        }
    }

    // Adds an Animator component to the GameObject if it doesn't already have one,
    // and assigns a new AnimatorOverrideController based on the provided base controller.
    private void EnsureAnimatorComponent(GameObject go)
    {
        Animator animator = go.GetComponent<Animator>();
        if (animator == null)
        {
            animator = go.AddComponent<Animator>();
        }

        // If the animator doesn't have a runtime controller or isn't already an override, create one.
        if (animator.runtimeAnimatorController == null || !(animator.runtimeAnimatorController is AnimatorOverrideController))
        {
            if (baseAnimatorController != null)
            {
                AnimatorOverrideController overrideController = new AnimatorOverrideController(baseAnimatorController);
                animator.runtimeAnimatorController = overrideController;
            }
            else
            {
                Debug.LogError("Base Animator Controller is not assigned on " + go.name);
            }
        }
    }

    // Overrides the "Animation" clip in the AnimatorOverrideController and plays the state.
    private void PlayAnimation(GameObject go, AnimationClip clip)
{
    Animator animator = go.GetComponent<Animator>();
    if (animator != null && clip != null)
    {
        // Ensure the animator has an override controller
        if (!(animator.runtimeAnimatorController is AnimatorOverrideController))
        {
            animator.runtimeAnimatorController = new AnimatorOverrideController(baseAnimatorController);
        }

        AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

        if (overrideController != null)
        {
            // Print all available animation keys
            Debug.Log("Available animation states:");
            foreach (var originalClip in overrideController.runtimeAnimatorController.animationClips)
            {
                Debug.Log("Found clip: " + originalClip.name);
            }

            // Check if "Animation" exists
            if (overrideController.runtimeAnimatorController.animationClips.Length > 0)
            {
                // Try assigning the new clip
                overrideController["Animation"] = clip;
                animator.Play("Animation", 0, 0f);
                animator.Update(0); // Force refresh
                Debug.Log("Playing animation: " + overrideController["Animation"] + " on " + go.name);
            }
            else
            {
                Debug.LogError("Error: 'Animation' state is missing from the Animator Controller!");
            }
        }
        else
        {
            Debug.LogError("Error: Failed to get AnimatorOverrideController on " + go.name);
        }
    }
}



}
