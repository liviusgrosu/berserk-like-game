using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillObjective : QuestObjective
{
    public string Enemy;
    public int Amount;

    public KillObjective(string title, string enemy, string amount)
    {
        // Kill an enemy/enemies objective
        Title = title;
        Enemy = enemy;
        Amount = Int32.Parse(amount);
        ObjectiveType = Type.Kill;
    }
}
