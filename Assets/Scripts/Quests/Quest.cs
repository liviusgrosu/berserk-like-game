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
    private List<QuestObjective> _currentObjectives;
    public string[] ItemRewards;
    public int SoulReward;
    public enum Status
    {
        NotStarted,
        Active,
        Completed,
        Failed
    };
    [HideInInspector]
    public Status CurrentStatus;

    public Quest(string title, string[] itemRewards, string soulReward)
    {
        // Instantiate the quest base parameters
        Title = title;
        ItemRewards = itemRewards;
        // JSON stores it as a string so needs to be converted to an int
        SoulReward = Int32.Parse(soulReward);
        Objectives = new List<QuestObjective>();
    }
}
