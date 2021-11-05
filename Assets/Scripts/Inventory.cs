using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using PlayerData;


public class Inventory : MonoBehaviour
{
    // Contains the consumable and count
    public List<ConsuambleInventory> ConsumableInventory;

    void Awake()
    {
        // Initialize the consumables list
        ConsumableInventory = new List<ConsuambleInventory>();

        Load();
    }

    public void AddConsumable(ConsumableItem consumable, int amount)
    {
        for(int i = 0; i < ConsumableInventory.Count; i++)
        {
            // If the consumable exists in the inventory then update the count
            if (consumable.name == ConsumableInventory[i].Consumable.name)
            {
                ConsumableInventory[i].Count += amount;
                return;
            }
        }

        // Add a new consumable item
        ConsumableInventory.Add(new ConsuambleInventory(consumable.GetComponent<ConsumableItem>(), amount));
    }

    private void Load()
    {
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/consumables.inventory"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/consumables.inventory", FileMode.Open);
            List<ConsuamblesData> ConsuamblesFileData = (List<ConsuamblesData>)bf.Deserialize(file);
            file.Close();

            foreach(ConsuamblesData consumable in ConsuamblesFileData)
            {
                GameObject item = Resources.Load<GameObject>($"Prefabs/Consumables/{consumable.Name}");
                AddConsumable(item.GetComponent<ConsumableItem>(), consumable.Quantity);
            }
        }
        else
        {
            // Add example consuambles
            GameObject item = Resources.Load<GameObject>("Prefabs/Consumables/Health Consumable Item");
            AddConsumable(item.GetComponent<ConsumableItem>(), 5);

            // item = Resources.Load<GameObject>("Prefabs/Consumables/Stamina Consumable Item");
            // AddConsumable(item.GetComponent<ConsumableItem>(), 4);

            item = Resources.Load<GameObject>("Prefabs/Consumables/Stamina Regeneration Consumable Item");
            AddConsumable(item.GetComponent<ConsumableItem>(), 4);
        }
    }

    private void Save()
    {
        // Remove data incase of change inventory
        List<ConsuamblesData> ConsuamblesFileData = new List<ConsuamblesData>();

        // Convert the consumable inventory to a saveable data structure
        foreach(ConsuambleInventory consumable in ConsumableInventory)
        {
            ConsuamblesFileData.Add(new ConsuamblesData(consumable.Consumable.name, consumable.Count));
        }

        // Save consumables inventory
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/consumables.inventory");
        bf.Serialize(file, ConsuamblesFileData);
        file.Close();
    }

    void Update()
    {
        // Add example consuambles
        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject item = Resources.Load<GameObject>("Prefabs/Consumables/Health Consumable Item");
            AddConsumable(item.GetComponent<ConsumableItem>(), 5);

            item = Resources.Load<GameObject>("Prefabs/Consumables/Stamina Consumable Item");
            AddConsumable(item.GetComponent<ConsumableItem>(), 4);

            item = Resources.Load<GameObject>("Prefabs/Consumables/Stamina Regeneration Consumable Item");
            AddConsumable(item.GetComponent<ConsumableItem>(), 4);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
