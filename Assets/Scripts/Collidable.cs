using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Collidable : MonoBehaviour
{
    public ContactFilter2D filter;

    [SerializeField]
    private BoxCollider2D boxCollider;
    private Collider2D[] hits = new Collider2D[10];


    protected virtual void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    protected virtual void Update()
    {
        // Collision Work
        boxCollider.OverlapCollider(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            OnCollide(hits[i]);

            // Clean array
            hits[i] = null;
        }
    }



    protected virtual void OnCollide(Collider2D coll)
    {
        Debug.Log("OnCollide was not implemented in: " + this.name);
    }
}
