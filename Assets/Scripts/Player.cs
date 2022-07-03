using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    /// <summary>
    /// float value: percentage of current health vs max health
    /// </summary>
    public static event Action<float> OnHealthChanged;

    public static event Action OnDeath;

    private bool dead = false;

    public bool isDead { get => dead; }

    private void FixedUpdate()
    {
        if (dead) return;
        if (GameManager.instance.textInputManager.isActive) return;

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        UpdateMotor(new Vector3(x, y, 0));
    }


    public override void ReceiveDamage(Damage dmg)
    {
        base.ReceiveDamage(dmg);

        Debug.Log("Hp left: " + hitpoints);

        float percentage = (float)hitpoints / (float)maxHitpoints;

        Debug.Log("Hp perc: " + percentage);

        OnHealthChanged?.Invoke(percentage);
    }

    protected override void Death()
    {
        dead = true;
        gameObject.SetActive(false);
        OnDeath?.Invoke();
    }
}
