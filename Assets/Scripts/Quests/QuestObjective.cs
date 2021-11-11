using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QuestObjective
{
    public string Title;
    public enum Type
    {
        Kill,
        Item,
        Interact,
        Enter,
        Talk
    };
    public Type ObjectiveType;
    public int Order;
}
