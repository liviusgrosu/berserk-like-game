using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : MonoBehaviour
{
    public List<ConsuambleInventory> Consumables;

    public GameObject healthPotion;

    void Awake()
    {
        Consumables = new List<ConsuambleInventory>();
        AddConsumable(healthPotion.GetComponent<ConsumableItem>(), 5);
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

        Consumables.Add(new ConsuambleInventory(healthPotion.GetComponent<ConsumableItem>(), amount));
    }
}
