using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestrian : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float minIdleTime = 5f;
    public float maxIdleTime = 40f;

    private Vector2 targetPosition;
    private PedestrianZone zone;

    private Animator animator;
    private SpriteRenderer sr;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        // 🔥 Randomize animation start offset
        if (animator != null)
        {
            animator.Play(0, -10, Random.value); 
            animator.speed = Random.Range(0.8f, 1.2f);
            // Random.value = 0 → 1 (random point in animation)
        }
    }

     void Update()
    {
        UpdateSortingOrder();
    }

    void UpdateSortingOrder()
    {
        if (sr != null)
        {
            sr.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }

    public void Init(PedestrianZone z)
    {
        zone = z;
        StartCoroutine(BehaviorLoop());
    }

    IEnumerator BehaviorLoop()
    {
        while (true)
        {
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);

            targetPosition = zone.GetRandomPoint();

            while (Vector2.Distance(transform.position, targetPosition) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );

                yield return null;
            }
        }
    }
}
