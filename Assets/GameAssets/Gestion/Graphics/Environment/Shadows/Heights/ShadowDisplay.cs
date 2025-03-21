using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDisplay : MonoBehaviour
{
     public int numberOfClones = 7;
    public float verticalOffset = -0.5f;
    private List<GameObject> clones = new List<GameObject>();

    void Start()
    {
        CreateClones();
    }


    void CreateClones()
    {
        for (int i = 0; i < numberOfClones; i++)
        {
            GameObject clone = Instantiate(gameObject, transform.position, Quaternion.identity, transform.parent);
            Destroy(clone.GetComponent<ShadowDisplay>()); // Prevent duplicate cloning script
            
            Vector3 newPosition = transform.position + new Vector3(0, verticalOffset * (i + 1), 0);
            clone.transform.position = newPosition;
            
            SpriteRenderer sr = clone.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = - (i + 1); // Set Z index order
            }
            
            clones.Add(clone);
        }
    }

}
