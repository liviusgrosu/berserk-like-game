using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipmentStats
{
    // Todo: save/load feature
    // Todo: base stats
    public float _durability;

    public EquipmentStats()
    {
        _durability = 100f;
    }
}