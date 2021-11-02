using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//public class PlayerEquipments : MonoBehaviour, IEquipment

public class PlayerEquipments : MonoBehaviour
{
    public Transform EquipmentParent;
    public List<string> StartingEquipment;
    private List<string> _equipmentCarrying;
    [HideInInspector]
    public List<GameObject> EquipmentIntances;
    // --- TEMP START ---
    public string TempCurrentWeaponName;
    private GameObject CurrentWeapon;
    // --- TEMP END ---
    public Transform Hand;
    private EntityCombat _EntityCombat;

    void Awake()
    {
        EquipmentIntances = new List<GameObject>();
        _EntityCombat = GetComponent<EntityCombat>();
        //  Create a new list regardless if its empty
        if (StartingEquipment == null)
        {
            StartingEquipment = new List<string>();
        }
        Load();
        InstantiateEquipment();
    }

    void Load()
    {
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/Equipments/equipment.inventory"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/Equipments/equipment.inventory", FileMode.Open);
            _equipmentCarrying = (List<string>)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // Create new equipment save
            _equipmentCarrying = StartingEquipment;
            Save();
        }
    }

    void Save()
    {
        // Save current equipment
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/Equipments/save.equipment");
        bf.Serialize(file, _equipmentCarrying);
    }

    void InstantiateEquipment()
    {
        foreach(String equipmentName in _equipmentCarrying)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Equipments/{equipmentName}");

            if (prefab != null)
            {
                // Create the instance and load its stats
                GameObject instantiatedObj = Instantiate(prefab, EquipmentParent.position, prefab.transform.rotation);
                instantiatedObj.name = equipmentName;
                instantiatedObj.transform.parent = EquipmentParent;
                // Disable some components 
                instantiatedObj.GetComponent<MeshCollider>().enabled = false;
                instantiatedObj.GetComponent<MeshRenderer>().enabled = false;
                // Load the stats
                instantiatedObj.GetComponent<IEquipment>().Load();
                EquipmentIntances.Add(instantiatedObj);

                // --- TEMP START ---
                if (instantiatedObj.name.Contains(TempCurrentWeaponName))
                {
                    CurrentWeapon = instantiatedObj;
                    Equip();
                }
                // --- TEMP END ---
            }
        }
    }

    void Equip()
    {
        CurrentWeapon.GetComponent<MeshCollider>().enabled = true;
        CurrentWeapon.GetComponent<MeshRenderer>().enabled = true;

        CurrentWeapon.transform.position = Hand.position;
        CurrentWeapon.transform.parent = Hand;
    }

    void Unequip()
    {

    }

    void OnApplicationQuit()
    {
        foreach(GameObject equipment in EquipmentIntances)
        {
            // Save equipment
            equipment.GetComponent<WeaponEquipment>().Save();
        }
    }

    public WeaponStats GetCurrentWeaponStats()
    {
        return CurrentWeapon.GetComponent<WeaponEquipment>().Stats;
    }
}
