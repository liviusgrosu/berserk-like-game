using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public GameObject ModelPrefab;
    public string ItemName;

    private EquipmentStats _stats;

    [HideInInspector]
    public enum EquipmentType
    {
        Weapon,
        Shield
    }
    public EquipmentType Type;

    public void Init()
    {
        Save();
    }

    public void Load() 
    {
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/{ItemName}.equipment"))
        {
            // Open the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{ItemName}.equipment", FileMode.Open);
            switch (Type)
            {
                case (EquipmentType.Weapon):
                    // Load the weapon stats
                    _stats = (WeaponStats)bf.Deserialize(file);
                    break;
                case (EquipmentType.Shield):
                    // Load the shield stats
                    _stats = (ShieldStats)bf.Deserialize(file);
                    break;
                default:
                    break; 
            }
            
            file.Close();
        }
        else
        {
            switch (Type)
            {
                case (EquipmentType.Weapon):
                    // Initialize the weapon stats
                    _stats = new WeaponStats();
                    break;
                case (EquipmentType.Shield):
                    // Initialize the shields stats
                    _stats = new ShieldStats();
                    break;
                default:
                    break; 
            }
            
            // Save new equipment
            Save();
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/{ItemName}.equipment");
        // Serialize the equipment and save it to its respective file
        switch (Type)
        {
            case (EquipmentType.Weapon):
                bf.Serialize(file, (WeaponStats)_stats);
                break;
            case (EquipmentType.Shield):
                bf.Serialize(file, (ShieldStats)_stats);
                break;
            default:
                break; 
        }
        
        file.Close();
    }
}