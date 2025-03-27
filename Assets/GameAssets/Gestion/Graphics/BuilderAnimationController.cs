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

    // Keep track of each builder's last sprite name
    private Dictionary<Builder, string> builderSpriteMap = new Dictionary<Builder, string>();

    // Mapping from sprite names to their corresponding animation clips
    private Dictionary<string, AnimationClip> spriteToAnim;

    void Start() 
    {
        // Create the sprite to animation mapping
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

                    // If this sprite has an animation, ensure the Animation component exists and play it
                    if (spriteToAnim.ContainsKey(sr.sprite.name))
                    {
                        EnsureAnimationComponent(builder.gameObject);
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
                    Debug.Log("AAAAAAAA");
                    builderSpriteMap[builder] = currentSpriteName;
                    Animation anim = builder.GetComponent<Animation>();

                    // Stop any current animation if present
                    if (anim != null)
                    {
                        anim.Stop();
                    }

                    // If the new sprite corresponds to an animation, play it
                    if (spriteToAnim.ContainsKey(currentSpriteName))
                    {
                        EnsureAnimationComponent(builder.gameObject);
                        PlayAnimation(builder.gameObject, spriteToAnim[currentSpriteName]);
                    }
                }
            }
        }
    }

    // Adds an Animation component to the GameObject if it doesn't already have one
    private void EnsureAnimationComponent(GameObject go)
    {
        if (go.GetComponent<Animation>() == null)
        {
            go.AddComponent<Animation>();
        }
    }

    // Adds and plays the given animation clip on loop
    private void PlayAnimation(GameObject go, AnimationClip clip)
    {
        Animation anim = go.GetComponent<Animation>();
        if (anim != null && clip != null)
        {
            // Add the clip if it hasn't been added yet
            if (anim.GetClip(clip.name) == null)
            {
                anim.AddClip(clip, clip.name);
            }
            anim.clip = clip;
            anim.wrapMode = WrapMode.Loop;
            anim.Play();
        }
    }
}
