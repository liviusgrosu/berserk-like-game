using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public int SoulCount;
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
