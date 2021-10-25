using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public int Experience;
    public int Gems;

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
            (int Experience, int Gems) lootData = ((int, int))bf.Deserialize(file);
            
            // Assign new data
            Experience = lootData.Experience;
            Gems = lootData.Gems;
            
            file.Close();
        }
        else 
        {
            // Default values
            Experience = 500;
            Gems = 100;
        }
    }
    private void Save()
    {
        // Save loot
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/loot.inventory");
        bf.Serialize(file, (Experience, Gems));
        file.Close();
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
