using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;


public class QuestManager : MonoBehaviour
{
    public List<Quest> AllQuests;
    public List<Quest> ActiveQuests;
    public List<Quest> CompletedQuests;

    // TODO: add a save and load features for current and completed quests

    void Awake()
    {
        AllQuests = new List<Quest>();
        ActiveQuests = new List<Quest>();
        CompletedQuests = new List<Quest>();

        DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("./Assets/JSON/quests.json"));
        DataTable questDataTable = dataSet.Tables["Quests"];
        DataTable objectiveDataTable = dataSet.Tables["Objectives"];

        // Go through the quest datatable
        foreach(DataRow questRow in questDataTable.Rows)
        {
            Quest currentQuest = new Quest(
                                    (string)questRow["Title"],
                                    (string[])questRow["ItemRewards"], 
                                    (string)questRow["SoulReward"]
                                    );

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
                        currentQuest.Objectives.Add(GetObjective(objectiveRow, objectiveOrder[objIdx]));
                    }

                    if ((string)questRow["TriggerObjective"] == (string)objectiveRow["Title"])
                    {
                        currentQuest.TriggerObjective = GetObjective(objectiveRow, "-1");
                    }
                }
            }

            // Add this to the current quests
            AllQuests.Add(currentQuest);
        }

        Load();

        // TEMP
        // foreach(Quest quest in ActiveQuests)
        // {
        //     quest.UpdateQuest();
        // }
    }

    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/quests.active");
        bf.Serialize(file, ActiveQuests);
        file.Close();
    }

    private void Load()
    {
        if (File.Exists($"{Application.persistentDataPath}/quests.save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/quests.active", FileMode.Open);
            ActiveQuests = (List<Quest>)bf.Deserialize(file);
            file.Close();
        }
    }

    private QuestObjective GetObjective(DataRow objective, string order)
    {
        QuestObjective questObjective = null;
        // Add the objective depending on its type
        switch((string)objective["Type"])
        {
            case "Kill":
                // Add a kill objective
                questObjective = new KillObjective(
                                                    (string)objective["Title"], 
                                                    (string)objective["Enemy"], 
                                                    (string)objective["Amount"],
                                                    order
                                                    );
                break;
            case "Item":
                // Add a find item objective
                questObjective = new ItemObjective(
                                                    (string)objective["Title"], 
                                                    (string)objective["Item"], 
                                                    (string)objective["Amount"],
                                                    order
                                                    );
                break;
            case "Talk":
                // Add a talk objective
                questObjective = new TalkObjective(
                                                    (string)objective["Title"], 
                                                    (string)objective["NPC"],
                                                    order
                                                    );
                break;
        }

        return questObjective;
    }

    public void TriggerEvent(QuestObjective.Type eventType, string eventName)
    {
        foreach(Quest quest in AllQuests.Except(ActiveQuests))
        {
            string triggerObjective = quest.TriggerObjective.Title;
            if (eventType == quest.TriggerObjective.ObjectiveType)
            {
                // If the trigger is a talk objective
                if (eventType == QuestObjective.Type.Talk)
                {
                    TalkObjective talkObjective = (TalkObjective)quest.TriggerObjective;
                    if (talkObjective.NPC == eventName)
                    {
                        ActiveQuests.Add(quest);
                        quest.UpdateQuest();
                        return;
                    }
                }
            }
        }

        // Search through the current objectives and check that this event modifies/completes it
        foreach(Quest quest in ActiveQuests)
        {
            foreach(QuestObjective objective in quest.CurrentObjective)
            {
                if (eventType == objective.ObjectiveType)
                {
                    if (eventType == QuestObjective.Type.Kill)
                    {
                        // Modify the kill objective
                        KillObjective killObjective = ((KillObjective)objective); 
                        if (killObjective.Enemy == eventName)
                        {
                            // If the enemy killed is part of the objective then modify it
                            killObjective.CurrentAmount++;
                            if (killObjective.CurrentAmount == killObjective.Amount)
                            {
                                // Kill objective is complete
                                quest.CurrentObjective.Remove(objective);
                                Debug.Log($"Kill {eventName} - objective complete");
                                break;
                            }
                        }
                    }
                    else if (eventType ==  QuestObjective.Type.Talk)
                    {
                        // Modify the kill objective
                        TalkObjective talkObjective = ((TalkObjective)objective); 
                        if (talkObjective.NPC == eventName)
                        {
                            // Complete the objective if the right NPC is talked to
                            quest.CurrentObjective.Remove(objective);
                            Debug.Log($"Talked to {eventName} - objective complete");
                            break;
                        }
                    }
                } 
            }
            // TODO: store the completed quest to use a trigger for new ones

            quest.UpdateQuest();
        }
    }
}
