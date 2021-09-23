using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public List<ConsuambleInventory> Consumables;

    public GameObject healthPotion, staminaPotion, staminaRegenerationPotion;

    void Awake()
    {
        Consumables = new List<ConsuambleInventory>();
        AddConsumable(healthPotion.GetComponent<ConsumableItem>(), 5);
        AddConsumable(staminaPotion.GetComponent<ConsumableItem>(), 3);
        AddConsumable(staminaRegenerationPotion.GetComponent<ConsumableItem>(), 1);
    }

    public void AddConsumable(ConsumableItem consumable, int amount)
    {
        for(int i = 0; i < Consumables.Count; i++)
        {
            if (consumable.name == Consumables[i].Consumable.name)
            {
                Consumables[i].Count += amount;
                return;
            }
        }

        Consumables.Add(new ConsuambleInventory(consumable.GetComponent<ConsumableItem>(), amount));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddConsumable(healthPotion.GetComponent<ConsumableItem>(), 5);
            AddConsumable(staminaPotion.GetComponent<ConsumableItem>(), 3);
            AddConsumable(staminaRegenerationPotion.GetComponent<ConsumableItem>(), 1);
        }
    }
}
