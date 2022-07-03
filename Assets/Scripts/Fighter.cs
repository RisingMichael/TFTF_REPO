using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    // Health fields
    public int hitpoints = 10;
    public int maxHitpoints = 10;
    public float pushRecoverySpeed = 0.2f;


    // Immunity
    protected float immuneTime = 1.0f;
    protected float lastImmune;


    // Push
    protected Vector3 pushDirection;


    // All fighters receive Damage / Die
    public virtual void ReceiveDamage(Damage dmg)
    {
        if(Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoints -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;


            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 40, Color.red, transform.position, Vector3.zero, 0.5f);


            // Die if hitpoints depleted
            if(hitpoints <= 0)
            {
                hitpoints = 0;
                Death();
            }
        }
    }

    protected virtual void Death()
    {

    }
}
