using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface WeaponStruct
{
    public const int uses = 1;
    public const int value = 1;
    public const float damage = 0;
    public const float pushForce = 2f;
    public const float cooldown = 0.3f;

    public const bool ranged = false;

    public const Sprite sprite = null;
    public const Sprite projectileSprite = null;
}
