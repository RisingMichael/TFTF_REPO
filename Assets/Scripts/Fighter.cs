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
    [SerializeField]
    protected List<DamageType> immunities;

    protected SpriteRenderer spriteRenderer;
    protected bool flickering = false;
    protected float lastFlicker;
    protected float flickerTime = 0.08f;


    protected AudioSource audioPlayer;

    // Push
    protected Vector3 pushDirection;








    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioPlayer = gameObject.AddComponent<AudioSource>();
        audioPlayer.volume = 0.7f;
    }





    // All fighters receive Damage / Die
    public virtual void ReceiveDamage(Damage dmg)
    {
        if (immunities.Contains(dmg.damageType))
            dmg.damageAmount = 0;
            
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitpoints -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;


            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 40, Color.red, transform.position, Vector3.zero, 0.5f);

            flickering = true;


            PlayHitSound();

            // Die if hitpoints depleted
            if(hitpoints <= 0)
            {
                hitpoints = 0;
                Death();
            }
        }
    }


    protected virtual void Update()
    {
        if(flickering)
        {
            if (Time.time - lastImmune > immuneTime)
                StopFlickering();
            else
                Flicker();
        }
    }

    public virtual void Heal(int amount)
    {
        hitpoints += amount;

        if (hitpoints > maxHitpoints) hitpoints = maxHitpoints;
    }



    protected void Flicker()
    {
        if (Time.time - lastFlicker > flickerTime)
        {
            lastFlicker = Time.time;
            if (spriteRenderer.color == Color.grey)
                spriteRenderer.color = Color.white;
            else
                spriteRenderer.color = Color.grey;
        }
    }


    protected void StopFlickering()
    {
        flickering = false;
        spriteRenderer.color = Color.white;
    }


    private void PlayHitSound()
    {
        audioPlayer.Stop();
        audioPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds\\Hit_Effect"));
    }

    public void PlaySwingSound()
    {
        audioPlayer.Stop();
        audioPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds\\Sword_swing"));
    }

    public void PlayShotSound()
    {
        audioPlayer.Stop();
        audioPlayer.PlayOneShot(Resources.Load<AudioClip>("Sounds\\Arrow_Shot"));
    }

    protected virtual void Death() { }
}
