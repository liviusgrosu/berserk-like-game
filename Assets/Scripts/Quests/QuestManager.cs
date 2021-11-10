using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class QuestManager : MonoBehaviour
{
    public List<Quest> Quests;
    void Awake()
    {
        Quests = new List<Quest>();

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("./Assets/JSON/temp.json"));
        DataTable questDataTable = dataSet.Tables["Quests"];
        DataTable objectiveDataTable = dataSet.Tables["Objectives"];

        foreach(DataRow questRow in questDataTable.Rows)
        {
            if ((string)questRow["Status"] == "NotStarted")
            {
                // Don't add this quest to the current quests
                continue;
            }
            Quest currentQuest = new Quest(
                                    (string)questRow["Title"], 
                                    (string)questRow["Description"],
                                    (string[])questRow["ItemRewards"], 
                                    (string)questRow["SoulReward"],
                                    (string)questRow["Status"]
                                    );

            // TODO: optimize this search
            // Go through all the objectives of the quests and the corresponding objective in the JSON objective
            foreach(string objectiveName in (string[])questRow["Objectives"])
            {
                foreach(DataRow objectiveRow in objectiveDataTable.Rows)
                {
                    if (objectiveName == (string)objectiveRow["Title"])
                    {
                        // Add the objective depending on its type
                        switch((string)objectiveRow["Type"])
                        {
                            case "Kill":
                                // Add a kill objective
                                KillObjective killObjective = new KillObjective(
                                                                    (string)objectiveRow["Title"], 
                                                                    (string)objectiveRow["Enemy"], 
                                                                    (string)objectiveRow["Amount"]
                                                                    );
                                currentQuest.Objectives.Add(killObjective);
                                break;
                            case "Item":
                                // Add a find item objective
                                ItemObjective itemObjective = new ItemObjective(
                                                                    (string)objectiveRow["Title"], 
                                                                    (string)objectiveRow["Item"], 
                                                                    (string)objectiveRow["Amount"]
                                                                    );
                                currentQuest.Objectives.Add(itemObjective);
                                break;
                            case "Talk":
                                // Add a talk objective
                                TalkObjective talkObjective = new TalkObjective(
                                                                (string)objectiveRow["Title"], 
                                                                (string)objectiveRow["NPC"]
                                                                );
                                currentQuest.Objectives.Add(talkObjective);
                                break;
                        }
                    }
                }
            }

            Quests.Add(currentQuest);
        }
    }
}
