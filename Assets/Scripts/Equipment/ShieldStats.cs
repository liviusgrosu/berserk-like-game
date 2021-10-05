using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShieldStats : EquipmentStats
{
    public float _defence;

    public ShieldStats() : base()
    {
        _defence = 1.0f;
    }
}