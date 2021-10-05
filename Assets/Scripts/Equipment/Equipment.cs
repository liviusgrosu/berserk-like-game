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
        Armor
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
                    Debug.Log("Got weapon save");
                    EquipmentStats save = (EquipmentStats)bf.Deserialize(file);
                    Debug.Log($"{ItemName} - durability: {_stats._durability}, damage:  {((WeaponStats)_stats)._damage}, attack speed: {((WeaponStats)_stats)._attackSpeed}");
                    break;
                case (EquipmentType.Armor):
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
                    // Init the weapon stats
                    _stats = new WeaponStats();
                    Debug.Log($"{ItemName} - durability: {_stats._durability}, damage:  {((WeaponStats)_stats)._damage}, attack speed: {((WeaponStats)_stats)._attackSpeed}");
                    break;
                case (EquipmentType.Armor):
                default:
                    break; 
            }
            // Set default states
            
            Save();
        }
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/{ItemName}.equipment");
        bf.Serialize(file, _stats);
        file.Close();
    }
}