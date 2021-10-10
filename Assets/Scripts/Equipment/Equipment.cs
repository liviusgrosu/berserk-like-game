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
    
    public EquipmentStats Stats;

    [HideInInspector]
    public enum EquipmentType
    {
        Weapon,
        Shield
    }
    public EquipmentType Type;

    public void Load() 
    {
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/{gameObject.name}.equipmentStats"))
        {
            // Open the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{gameObject.name}.equipmentStats", FileMode.Open);
            switch (Type)
            {
                case (EquipmentType.Weapon):
                    // Load the weapon stats
                    Stats = (WeaponStats)bf.Deserialize(file);
                    break;
                case (EquipmentType.Shield):
                    // Load the shield stats
                    Stats = (ShieldStats)bf.Deserialize(file);
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
                    Stats = new WeaponStats();
                    break;
                case (EquipmentType.Shield):
                    // Initialize the shields stats
                    Stats = new ShieldStats();
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
        FileStream file = File.Create($"{Application.persistentDataPath}/{gameObject.name}.equipmentStats");
        // Serialize the equipment and save it to its respective file
        switch (Type)
        {
            case (EquipmentType.Weapon):
                bf.Serialize(file, (WeaponStats)Stats);
                break;
            case (EquipmentType.Shield):
                bf.Serialize(file, (ShieldStats)Stats);
                break;
            default:
                break; 
        }
        
        file.Close();
    }
}