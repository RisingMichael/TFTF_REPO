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

    private bool isVolatile = true;

    public void Initialize(int damage, float pushForce,
        Vector3 moveDir, Sprite sprite)
    {
        this.damage = damage;
        this.pushForce = pushForce;
        this.moveDir = moveDir;
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
        if (coll.tag == "Fighter")
        {
            if (!isVolatile) return;
            if (coll.name == "Player") return;

            // Create new damage object, before sending it to the player
            Damage dmg = new Damage
            {
                damageAmount = damage,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);            
        }

        //destroy projectile if has hit something (inefficient but since it is a prototype a objectpool is not necessary)
        GetComponent<SpriteRenderer>().enabled = false;
        isVolatile = false;
    }

    IEnumerator SelfDestructCounter()
    {
        yield return new WaitForSeconds(selfDestructTime);

        Destroy(gameObject);
    }
}
