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
    
    [HideInInspector]
    public EquipmentStats Stats;

    [HideInInspector]
    public enum EquipmentType
    {
        Weapon,
        Shield
    }
    public EquipmentType Type;

    [HideInInspector]
    // List of upgrade ids from the skill tree
    public List<int> CurrentUpgradeIds;
    
    public void Save()
    {
        // --- Stats ---
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
        // --- Skills ---
        file = File.Create($"{Application.persistentDataPath}/{gameObject.name}.equipmentUpgrades");
        bf.Serialize(file, CurrentUpgradeIds);
        file.Close();
    }

    public void Load()
    {
        // --- Stats ---
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
        }
        // --- Skills ---
        if (File.Exists($"{Application.persistentDataPath}/{gameObject.name}.equipmentUpgrades"))
        {
            // Load the upgrade IDs if the file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{gameObject.name}.equipmentUpgrades", FileMode.Open);
            CurrentUpgradeIds = (List<int>)bf.Deserialize(file);
        }
        else 
        {
            // Create a new upgrade IDs list
            CurrentUpgradeIds = new List<int>();
        }
    }

    public void AddUpgrade(EquipmentUpgradeNode upgradeNode)
    {
        // Add upgrade upgrade and increment the respective stat
        CurrentUpgradeIds.Add(upgradeNode.ID);
        switch (upgradeNode.UpgradeName)
        {
            case "durability":
                Stats.Durability += upgradeNode.Amount;
                break;
            case "damage":
                ((WeaponStats)Stats).Damage += upgradeNode.Amount;
                break;
            case "attack speed":
                ((WeaponStats)Stats).AttackSpeed += upgradeNode.Amount;
                break;
            case "defence":
                ((ShieldStats)Stats).Defence += upgradeNode.Amount;
                break;
            default:
                break;
        }
    }

    public bool CheckIfUpgradeUnlocked(EquipmentUpgradeNode upgradeNode)
    {
        return CurrentUpgradeIds.Contains(upgradeNode.ID);
    }
}