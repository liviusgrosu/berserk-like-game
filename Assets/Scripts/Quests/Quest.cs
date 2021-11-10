using System.Collections;
using System.Collections.Generic;

public class Quest
{
    public string Title;
    public string Description;
    public List<QuestObjective> Objectives;
    public List<string> Rewards;
    public enum Status
    {
        NotStarted, 
        Pending,
        Finished,
        Failed
    }
    public Status CurrentStatus;
}
