using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkObjective : QuestObjective
{
    public string NPC;

    public TalkObjective(string title, string npc)
    {
        // Talk to an NPC objective
        Title = title;
        NPC = npc;
        ObjectiveType = Type.Talk;
    }
}
