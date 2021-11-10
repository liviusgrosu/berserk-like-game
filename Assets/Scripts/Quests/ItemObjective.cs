using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObjective : QuestObjective
{
    public string Item;
    public int Amount;

    public ItemObjective(string title, string item, string amount)
    {
        // Find an item objective
        Title = title;
        Item = item;
        Amount = Int32.Parse(amount);
        ObjectiveType = Type.Item;
    }
}
