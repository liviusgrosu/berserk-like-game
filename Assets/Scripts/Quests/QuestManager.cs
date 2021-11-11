using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class QuestManager : MonoBehaviour
{
    public List<Quest> ActiveQuests;
    public List<Quest> CompletedQuests;

    public string TempCurrentQuestName;

    // TODO: add a save and load features for current and completed quests

    void Awake()
    {
        ActiveQuests = new List<Quest>();
        CompletedQuests = new List<Quest>();

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("./Assets/JSON/quests.json"));
        DataTable questDataTable = dataSet.Tables["Quests"];
        DataTable objectiveDataTable = dataSet.Tables["Objectives"];

        foreach(DataRow questRow in questDataTable.Rows)
        {
            Quest currentQuest = new Quest(
                                    (string)questRow["Title"],
                                    (string[])questRow["ItemRewards"], 
                                    (string)questRow["SoulReward"]
                                    );

            // Don't process the quest in the database if its not currently active
            if (TempCurrentQuestName != currentQuest.Title)
            {
                continue;
            }

            // Get all the quest objectives and their order
            string[] objectives = (string[])questRow["Objectives"];
            string[] objectiveOrder = (string[])questRow["ObjectivesOrder"];
            // TODO: optimize this search
            // Go through all the objectives of the quests and the corresponding objective in the JSON objective
            for(int objIdx = 0; objIdx < objectives.Length; objIdx++)
            {
                // Go through the objective database and get the objective that matches the current index for the quest
                foreach(DataRow objectiveRow in objectiveDataTable.Rows)
                {
                    if (objectives[objIdx] == (string)objectiveRow["Title"])
                    {
                        // Add the objective depending on its type
                        switch((string)objectiveRow["Type"])
                        {
                            case "Kill":
                                // Add a kill objective
                                KillObjective killObjective = new KillObjective(
                                                                    (string)objectiveRow["Title"], 
                                                                    (string)objectiveRow["Enemy"], 
                                                                    (string)objectiveRow["Amount"],
                                                                    objectiveOrder[objIdx]
                                                                    );
                                currentQuest.Objectives.Add(killObjective);
                                break;
                            case "Item":
                                // Add a find item objective
                                ItemObjective itemObjective = new ItemObjective(
                                                                    (string)objectiveRow["Title"], 
                                                                    (string)objectiveRow["Item"], 
                                                                    (string)objectiveRow["Amount"],
                                                                    objectiveOrder[objIdx]
                                                                    );
                                currentQuest.Objectives.Add(itemObjective);
                                break;
                            case "Talk":
                                // Add a talk objective
                                TalkObjective talkObjective = new TalkObjective(
                                                                (string)objectiveRow["Title"], 
                                                                (string)objectiveRow["NPC"],
                                                                objectiveOrder[objIdx]
                                                                );
                                currentQuest.Objectives.Add(talkObjective);
                                break;
                        }
                    }
                }
            }

            // Add this to the current quests
            ActiveQuests.Add(currentQuest);
        }
    }

    void HandleTalkEvent(string npcName)
    {
        // TODO: check the not-started quests with the NPC and see if we can start it
        
    }
}
