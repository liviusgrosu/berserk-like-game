using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest
{
    // TODO: might need get; set;
    public string Title;
    public string Description;
    public string[] Objectives;
    public string[] ItemRewards;
    public int SoulReward;
    public enum Status
    {
        NonStarted,
        Pending,
        Finished,
        Failed
    };
    public Status CurrentStatus;

    public Quest(string title, string description, string[] itemRewards, string soulReward, string status)
    {
        Title = title;
        Description = description;
        ItemRewards = itemRewards;
        SoulReward = Int32.Parse(soulReward);

        Enum.TryParse(status, out CurrentStatus);
    }
}
