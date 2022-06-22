using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    // Damage struct
    public int damagePoint = 1;
    public int usesLeft = 1;
    public float pushForce = 2.0f;

    public bool ranged = false;

    private bool broken = false;

    private SpriteRenderer spriteRenderer;


    // Swing
    private Animator anim;
    private float cooldown = 0.3f;
    private float lastSwing;

    private Sprite projectileSprite = null;


    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }


    protected override void Update()
    {
        base.Update();

        if (broken)
            return;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
            }
        }
    }



    protected override void OnCollide(Collider2D coll)
    {
        // No damage if weapon is ranged
        if (ranged)
            return;

        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            // Create new damage object
            // Send to fighter that's hit
            Damage dmg = new Damage
            {
                damageAmount = damagePoint,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }


    private void Swing()
    {
        anim.SetTrigger("Swing");
        usesLeft--;
        if (usesLeft <= 0)
            Break();
    }


    /*
     *  Weapon breaks
     */
    private void Break()
    {
        usesLeft = 0;
        broken = true;
        spriteRenderer.enabled = false;
    }


    private void Shoot()
    {
        // TODO: Trigger shooting animation and launch projectile
    }


    public void InitializeWithNewWeapon(WeaponStruct weaponStruct)
    {
        // TODO: Implement
    }
}
