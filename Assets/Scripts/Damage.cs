using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Damage
{
    public Vector3 origin;
    public int damageAmount;
    public float pushForce;
    public DamageType damageType;
}

public enum DamageType
{
    Melee, Ranged
}