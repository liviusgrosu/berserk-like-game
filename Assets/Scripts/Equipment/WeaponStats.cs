using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats : EquipmentStats
{
    public float Damage;
    public float AttackSpeed;
    public float StaminaUse;

    public WeaponStats() : base()
    {
        Damage = 1.0f;
        AttackSpeed = 1.0f;
        StaminaUse = 2.0f;
    }
}