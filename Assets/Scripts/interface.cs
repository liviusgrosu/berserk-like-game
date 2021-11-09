using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

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
    void ProvideStats(EntityStats stats, Action<GameObject> destroyCallback);
    Sprite GetIcon();
}

public interface IInteract
{
    void Interact();
}