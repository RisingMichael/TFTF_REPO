using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public struct WeaponData
{
    public readonly string name;
    public readonly int uses;
    public readonly int value;
    public readonly int damage;
    public readonly float pushForce;
    public readonly float cooldown;

    public readonly bool ranged;

    public readonly Sprite sprite;
    public readonly Sprite projectileSprite;

    public WeaponData(string name, int uses, int value, int damage, float pushForce,
        float cooldown, bool ranged, Sprite sprite, Sprite projectileSprite)
    {
        this.name = name;
        this.uses = uses;
        this.value = value;
        this.damage = damage;
        this.pushForce = pushForce;
        this.cooldown = cooldown;
        this.ranged = ranged;
        this.sprite = sprite;
        this.projectileSprite = projectileSprite;
    }
}

public class Weapon : Collidable
{
    /// <summary>
    /// bool value: true if only uses change; change if the whole weapon changed
    /// </summary>
    public static event Action<bool> OnWeaponInfoChanged;

    // Damage struct
    public string weaponName;
    public int damagePoint;
    private int usesLeft;
    public float pushForce;

    public bool ranged;

    private bool broken = true;

    private SpriteRenderer spriteRenderer;

    //Reader
    private WeaponReader weaponReader;

    // Swing
    private Animator anim;
    private float cooldown = 0.3f;
    private float lastAttack;

    private Sprite projectileSprite = null;

    protected override void Awake()
    {
        base.Awake();
        weaponReader = new WeaponReader(this);
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //start with no weapon
        usesLeft = 0;
        broken = true;
        spriteRenderer.enabled = false;
        OnWeaponInfoChanged?.Invoke(false);
    }

    public int uses { get => usesLeft; }
    public bool isBroken { get => broken; }

    protected override void Update()
    {
        base.Update();
        
        if (GameManager.instance.textInputManager.isActive) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameManager.instance.textInputManager.ActivateTextInput();
        }

        if (broken) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.time - lastAttack > cooldown)
            {
                lastAttack = Time.time;

                if (ranged) Shoot();
                else Swing();
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
        StartCoroutine(DecreaseUses());
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

        GameObject projObj = Instantiate(Resources.Load<GameObject>("Prefabs\\Projectile"));
        projObj.transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f);

        Vector2 dir2 = Vector2.right;
        if (transform.parent.localScale.x < 0) dir2 = Vector2.left;

        Projectile proj = projObj.GetComponent<Projectile>();
        proj.Initialize(damagePoint, pushForce, new Vector3(dir2.x, dir2.y), projectileSprite);

        usesLeft--;
        Debug.Log("Uses Left: " + usesLeft);
        if (usesLeft <= 0) Break();
        OnWeaponInfoChanged?.Invoke(true);
    }

    public void InitializeWithNewWeapon(WeaponData weaponData)
    {
        weaponName = weaponData.name;
        damagePoint = weaponData.damage;
        usesLeft = weaponData.uses;
        broken = false;
        pushForce = weaponData.pushForce;
        cooldown = weaponData.cooldown;
        ranged = weaponData.ranged;

        projectileSprite = weaponData.projectileSprite;
        spriteRenderer.sprite = weaponData.sprite;

        spriteRenderer.enabled = true;

        OnWeaponInfoChanged?.Invoke(false);
    }

    IEnumerator DecreaseUses()
    {
        yield return new WaitForSeconds(cooldown);

        usesLeft--;
        Debug.Log("Uses Left: " + usesLeft);
        if (usesLeft <= 0) Break();
        OnWeaponInfoChanged?.Invoke(true);
    }
}
