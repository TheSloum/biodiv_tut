using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianZone : MonoBehaviour
{
    public GameObject prefab;
    public int minCount = 5;
    public int maxCount = 25;

    public Vector2 areaSize = new Vector2(10f, 5f);

    public Sprite[] possibleSprites; // assign 5 sprites here

    void Start()
    {
        int count = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPos = GetRandomPoint();
            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

            // Assign random sprite
            SpriteRenderer sr = obj.GetComponentInChildren<SpriteRenderer>();
sr.sprite = possibleSprites[Random.Range(0, possibleSprites.Length)];

            // Init wander behavior
            obj.GetComponent<Pedestrian>().Init(this);
        }
    }

    public Vector2 GetRandomPoint()
    {
        Vector2 center = transform.position;

        float x = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        float y = Random.Range(-areaSize.y / 2, areaSize.y / 2);

        return center + new Vector2(x, y);
    }

    // Optional: visualize zone in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
