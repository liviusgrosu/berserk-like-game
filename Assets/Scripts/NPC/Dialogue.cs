using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public string[] Lines;

    public struct QuestCondition
    {
        public QuestCondition(string title, string objective)
        {
            Title = title;
            Objective = objective;
        }
        public string Title;
        public string Objective;
    }

    public List<QuestCondition> QuestConditions;

    public Dialogue(string[] lines)
    {
        Lines = lines;
        QuestConditions = new List<QuestCondition>();
    }
}
