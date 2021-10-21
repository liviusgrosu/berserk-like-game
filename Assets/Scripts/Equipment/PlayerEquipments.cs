using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using UnityEngine;

public class PlayerEquipments : MonoBehaviour
{
    public Transform EquipmentParent;
    public List<string> StartingEquipment;
    private List<string> _equipmentCarrying;
    [HideInInspector]
    public List<GameObject> EquipmentIntances;
    // --- TEMP START ---
    public string TempCurrentEquipmentName;
    private GameObject TempCurrentEquipment;
    // --- TEMP END ---
    public Transform Hand;

    void Awake()
    {
        EquipmentIntances = new List<GameObject>();
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
        if (File.Exists($"{Application.persistentDataPath}/save.equipment"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/save.equipment", FileMode.Open);
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
        FileStream file = File.Create($"{Application.persistentDataPath}/save.equipment");
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
                GameObject instantiatedObj = Instantiate(prefab, EquipmentParent.position, Quaternion.identity);
                instantiatedObj.name = equipmentName;
                instantiatedObj.transform.parent = EquipmentParent;
                instantiatedObj.GetComponent<Equipment>().Load();
                EquipmentIntances.Add(instantiatedObj);

                // --- TEMP START ---
                if (instantiatedObj.name.Contains(TempCurrentEquipmentName))
                {
                    TempCurrentEquipment = instantiatedObj;
                    Equip();
                }
                // --- TEMP END ---
            }
        }
    }

    void Equip()
    {
        GameObject equipmentModel = Instantiate(TempCurrentEquipment.GetComponent<Equipment>().ModelPrefab, Hand.position, TempCurrentEquipment.GetComponent<Equipment>().ModelPrefab.transform.rotation);
        equipmentModel.transform.parent = Hand;
        // Give the weapon collider the equipment stats
        equipmentModel.GetComponent<WeaponCollider>().AssignStats(TempCurrentEquipment.GetComponent<Equipment>().Stats);
    }

    void OnApplicationQuit()
    {
        foreach(GameObject equipment in EquipmentIntances)
        {
            // Save equipment
            equipment.GetComponent<Equipment>().Save();
        }
    }
}
