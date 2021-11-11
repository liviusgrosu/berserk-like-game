using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TalkObjective : QuestObjective
{
    public string NPC;

    public TalkObjective(string title, string npc, string order)
    {
        // Talk to an NPC objective
        Title = title;
        NPC = npc;
        ObjectiveType = Type.Talk;
        Order = Int32.Parse(order);
    }
}
