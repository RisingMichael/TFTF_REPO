using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Collidable
{
    private int damage;
    private float pushForce;
    private Vector3 moveDir;

    private const float moveSpeed = 2.0f;

    private const float selfDestructTime = 5.0f;

    private const float deactivationTime = 0.1f;

    private bool isPiercing = false;

    private bool isVolatile = true;

    public void Initialize(int damage, float pushForce,
        Vector3 moveDir, Sprite sprite)
    {
        this.damage = damage;
        this.pushForce = pushForce;
        this.moveDir = moveDir;
        if (damage >= 30)
            isPiercing = true;
        GetComponent<SpriteRenderer>().sprite = sprite;
        StartCoroutine(SelfDestructCounter());
    }

    protected override void Awake()
    {
        base.Awake();
    } 

    protected override void Update()
    {
        base.Update();
        transform.position += moveDir * Time.deltaTime * moveSpeed;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Collectable") return;
        if (coll.tag == "Player") return;
        if (coll.tag == "Fighter")
        {
            if (!isVolatile) return;

            // Create new damage object, before sending it to the hit object
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce,
                damageType = DamageType.Ranged
            };

            coll.SendMessage("ReceiveDamage", dmg);            
        }

        //destroy projectile if has hit something (inefficient but since it is a prototype a objectpool is not necessary)
        // ^Only if projectile is not piercing
        if(!isPiercing)
            GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(DeactivateProjectile());
    }

    IEnumerator DeactivateProjectile()
    {
        yield return new WaitForSeconds(deactivationTime);
        isVolatile = false;
    }

    IEnumerator SelfDestructCounter()
    {
        yield return new WaitForSeconds(selfDestructTime);

        Destroy(gameObject);
    }
}
