using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipments : MonoBehaviour
{
    public List<Equipment> Equipment;

    private Equipment Sword;

    void Start()
    {
        // Load equipment stats
        foreach(Equipment equipment in Equipment)
        {
            equipment.Load();
        }
    }

    void OnApplicationQuit()
    {
        // Save equipment stats
        foreach(Equipment equipment in Equipment)
        {
            equipment.Save();
        }
    }
}
