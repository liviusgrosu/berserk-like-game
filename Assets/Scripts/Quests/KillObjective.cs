using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KillObjective : QuestObjective
{
    // Which enemy to kill
    public string Enemy;
    // How many to kill
    public int Amount;
    // How many were killed
    public int CurrentAmount;

    public KillObjective(string title, string enemy, string amount, string order)
    {
        // Kill an enemy/enemies objective
        Title = title;
        Enemy = enemy;
        Amount = Int32.Parse(amount);
        CurrentAmount = 0;
        ObjectiveType = Type.Kill;
        Order = Int32.Parse(order);
    }
}
