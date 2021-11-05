using UnityEngine;
using System.Collections;

//This is a basic interface with a single required
//method.
public interface IEquipment
{
    //EquipmentStats GetCurrentEquipmentStats();
    void Load();
    void Save();
}

public interface IEntity
{
    void RecieveHit(float damage);
}

public interface IStatEffect
{
    void ProvideStats(EntityStats stats);
}