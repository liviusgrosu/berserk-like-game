using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquipment : MonoBehaviour, IEquipment
{
    public WeaponStats Stats;

    [HideInInspector]
    // List of upgrade ids from the skill tree
    public List<int> CurrentUpgradeIds;
    
    public void Save()
    {
        // --- Stats ---
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentStats");
        // Serialize the equipment and save it to its respective file
        bf.Serialize(file, (WeaponStats)Stats);
        file.Close();
        // --- Skills ---
        file = File.Create($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentUpgrades");
        bf.Serialize(file, CurrentUpgradeIds);
        file.Close();
    }

    public void Load()
    {
        // --- Stats ---
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentStats"))
        {
            // Open the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentStats", FileMode.Open);
            Stats = (WeaponStats)bf.Deserialize(file);
            file.Close();
        }
        // --- Skills ---
        if (File.Exists($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentUpgrades"))
        {
            // Load the upgrade IDs if the file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/Equipments/Weapons/{gameObject.name}.equipmentUpgrades", FileMode.Open);
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
                Stats.Damage += upgradeNode.Amount;
                break;
            case "attack speed":
                Stats.AttackSpeed += upgradeNode.Amount;
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
