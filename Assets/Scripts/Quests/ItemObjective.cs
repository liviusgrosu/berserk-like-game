using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemObjective : QuestObjective
{
    public string Item;
    public int Amount;
    public int CurrentAmount;

    public ItemObjective(string title, string item, string amount, string order)
    {
        // Find an item objective
        Title = title;
        Item = item;
        Amount = Int32.Parse(amount);
        ObjectiveType = Type.Item;
        Order = Int32.Parse(order);
    }
}
