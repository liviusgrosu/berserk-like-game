using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [HideInInspector]
    public int SoulCount;

    [Tooltip("Various soul sizes")]
    [Header("Soul Prefabs")]
    public GameObject SmallSoulPrefab;
    public GameObject MediumSoulPrefab;
    public GameObject LargeSoulPrefab;

    [Tooltip("Set ranges between soul sizes to spawn the correct size")]
    [Header("Soul Ranges")]
    public int MediumSoulStartAmount;
    public int LargeSoulStartAmount;

    void Awake()
    {
        Load();
    }

    private void Load()
    {
        if (File.Exists($"{Application.persistentDataPath}/loot.inventory"))
        {
            // Get the file and data
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/loot.inventory", FileMode.Open);
            SoulCount = (int)bf.Deserialize(file);            
            file.Close();
        }
        else 
        {
            // Default values
            SoulCount = 1000;
        }
    }
    private void Save()
    {
        // Save loot
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/loot.inventory");
        bf.Serialize(file, SoulCount);
        file.Close();
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
