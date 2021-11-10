using System;
using System.Collections;
using System.Collections.Generic;

public class Quest
{
    // TODO: might need get; set;
    public string Title;
    public string Description;
    public List<QuestObjective> Objectives;
    public string[] ItemRewards;
    public int SoulReward;
    public enum Status
    {
        NotStarted,
        Pending,
        Finished,
        Failed
    };
    public Status CurrentStatus;

    public Quest(string title, string description, string[] itemRewards, string soulReward, string status)
    {
        // Instantiate the quest base parameters
        Title = title;
        Description = description;
        ItemRewards = itemRewards;
        // JSON stores it as a string so needs to be converted to an int
        SoulReward = Int32.Parse(soulReward);

        // Get the status of the quest
        Enum.TryParse(status, out CurrentStatus);

        Objectives = new List<QuestObjective>();
    }
}
