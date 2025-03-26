using UnityEngine;

public class BuilderAnimationController : MonoBehaviour
{
    // Reference animation state names (make sure these match the state names in your Animator Controller)
    public string pizzeriaState = "pizzeriaAnim";
    public string thermState = "thermAnim";
    public string hydrauState = "hydrauAnim";
    public string jardinState = "jardinAnim";
    public string build1State = "build1Anim";
    public string build2State = "build2Anim";

    // Cache all builder objects in the scene.
    private Builder[] builders;

    void Start()
    {
        // Find all gameobjects with the Builder script.
        builders = GameObject.FindObjectsOfType<Builder>();
    }

    void Update()
    {
        foreach (Builder b in builders)
        {
            // Get the SpriteRenderer to check the current sprite
            SpriteRenderer sr = b.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                string spriteName = sr.sprite.name;
                string stateToPlay = "";

                // Determine which state to play based on the sprite's name
                switch (spriteName)
                {
                    case "PIZZERIA":
                        stateToPlay = pizzeriaState;
                        break;
                    case "centrale thermique":
                        stateToPlay = thermState;
                        break;
                    case "HYDRAULIQUE_COMPLET":
                        stateToPlay = hydrauState;
                        break;
                    case "JARDIN":
                        stateToPlay = jardinState;
                        break;
                    case "construction V1 type 2":
                        stateToPlay = build1State;
                        break;
                    case "grandeConstruction1":
                        stateToPlay = build2State;
                        break;
                    default:
                        // No matching animation state, you can add additional logic if needed.
                        break;
                }

                // If a matching state was found, play the animation using the Animator
                if (!string.IsNullOrEmpty(stateToPlay))
                {
                    Animator animator = b.GetComponent<Animator>();
                    if (animator != null)
                    {
                        animator.Play(stateToPlay);
                    }
                    else
                    {
                        Debug.LogWarning("GameObject " + b.name + " is missing an Animator component!");
                    }
                }
            }
        }
    }
}
