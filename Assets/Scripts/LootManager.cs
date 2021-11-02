using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public int Experience;
    public int Gems;
    public int Coins;

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
            (int Experience, int Gems, int Coins) lootData = ((int, int, int))bf.Deserialize(file);
            
            // Assign new data
            Experience = lootData.Experience;
            Gems = lootData.Gems;
            Coins = lootData.Coins;
            
            file.Close();
        }
        else 
        {
            // Default values
            Experience = 500;
            Gems = 100;
            Coins = 1000;
        }
    }
    private void Save()
    {
        // Save loot
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/loot.inventory");
        bf.Serialize(file, (Experience, Gems, Coins));
        file.Close();
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
