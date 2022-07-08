using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    // Experience
    public int xpValue = 1;

    // Logic
    public float triggerLength = 1.0f;
    public float chaseLength = 5.0f;

    private bool chasing;
    private bool collidingWithPlayer;

    private Vector3 startingPosition;

    // Hitbox
    public ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];


    protected override void Awake()
    {
        base.Awake();
        startingPosition = transform.position;
        // Get the hitbox(is the first child of the enemy object)
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
    }


    private void FixedUpdate()
    {
        // Check if player in range
        Transform playerTransform = GameManager.instance.player.transform;
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerLength)
                chasing = true;

            if(chasing)
            {
                if(!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }


        // Check for overlap
        collidingWithPlayer = false;
        hitBox.OverlapCollider(filter, hits);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
                collidingWithPlayer = true;

            // Clean array
            hits[i] = null;
        }
    }



    protected override void Death()
    {
        Destroy(gameObject);
        //GameManager.instance.ShowText("+" + xpValue.ToString() + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }
}
