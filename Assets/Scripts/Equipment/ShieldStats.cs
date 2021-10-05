using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShieldStats : EquipmentStats
{
    public float Defence;

    public ShieldStats() : base()
    {
        Defence = 1.0f;
    }
}