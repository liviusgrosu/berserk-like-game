using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    // TODO: might need get; set;
    public string Title;
    public List<QuestObjective> Objectives;
    public List<QuestObjective> CurrentObjective;
    public QuestObjective TriggerObjective;
    public string[] Conditions;
    private int _objectiveIndex;
    public string[] ItemRewards;
    public int SoulReward;

    public Quest(string title, string[] itemRewards, string soulReward, string[] conditions)
    {
        // Instantiate the quest base parameters
        Title = title;
        ItemRewards = itemRewards;
        // JSON stores it as a string so needs to be converted to an int
        SoulReward = Int32.Parse(soulReward);
        Objectives = new List<QuestObjective>();
        CurrentObjective = new List<QuestObjective>();
        TriggerObjective = new QuestObjective();
        Conditions = conditions;
    }

    public void UpdateQuest()
    {
        if (CurrentObjective.Count == 0)
        {
            // Update with new objectives
            _objectiveIndex++;

            // Update the objectives
            foreach(QuestObjective objective in Objectives)
            {
                if (objective.Order == _objectiveIndex)
                {
                    CurrentObjective.Add(objective);
                }
            }
        }
    }
}
