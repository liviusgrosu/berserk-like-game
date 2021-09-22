using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsuambleInventory
{
    public ConsumableItem Consumable;
    public int Count;

    public ConsuambleInventory(ConsumableItem item, int count)
    {
        Consumable = item;
        Count = count;
    }
}