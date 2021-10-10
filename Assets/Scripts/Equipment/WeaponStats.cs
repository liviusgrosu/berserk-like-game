using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponStats : EquipmentStats
{
    public float Damage;
    public float AttackSpeed;

    public WeaponStats() : base()
    {
        Damage = 1.0f;
        AttackSpeed = 1.0f;
    }
}