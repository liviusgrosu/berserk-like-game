using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string[] Lines;

    public struct QuestCondtion
    {
        public QuestCondtion(string title, string objective)
        {
            Title = title;
            Objective = objective;
        }
        string Title;
        string Objective;
    }
    public struct ItemCondition
    {
        public ItemCondition(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }
        string Name;
        int Amount;
    }

    public List<QuestCondtion> QuestConditions;
    public List<ItemCondition> ItemConditions;

    public Dialogue(string[] lines)
    {
        Lines = lines;
        QuestConditions = new List<QuestCondtion>();
        ItemConditions = new List<ItemCondition>();
    }
}
