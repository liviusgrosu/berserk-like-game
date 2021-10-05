using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class WeaponStats : EquipmentStats
{
    public float _damage;
    public float _attackSpeed;

    public WeaponStats() : base()
    {
        _damage = 1.0f;
        _attackSpeed = 1.0f;
    }
}