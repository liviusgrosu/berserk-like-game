using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;


public class QuestManager : MonoBehaviour
{
    public List<Quest> AllQuests;
    public List<Quest> ActiveQuests;
    public List<Quest> CompletedQuests;

    public List<GameObject> NPCEntites;

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
                                    (string)questRow["SoulReward"],
                                    (string[])questRow["Conditions"]
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
    }

    private void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/quests.active");
        // Save the active quests
        bf.Serialize(file, ActiveQuests);
        file = File.Create($"{Application.persistentDataPath}/quests.completed");
        // Save the completed quests
        bf.Serialize(file, CompletedQuests);
        file.Close();
    }

    private void Load()
    {
        if (File.Exists($"{Application.persistentDataPath}/quests.active") && File.Exists($"{Application.persistentDataPath}/quests.completed"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            // Load the active quests
            FileStream file = File.Open($"{Application.persistentDataPath}/quests.active", FileMode.Open);
            ActiveQuests = (List<Quest>)bf.Deserialize(file);
            // Load the completed quests
            file = File.Open($"{Application.persistentDataPath}/quests.completed", FileMode.Open);
            CompletedQuests = (List<Quest>)bf.Deserialize(file);
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
        // Check to see if a quest can be added
        foreach(Quest quest in AllQuests)
        {
            string triggerObjective = quest.TriggerObjective.Title;
            if (eventType == quest.TriggerObjective.ObjectiveType)
            {
                // If the trigger is a talk objective
                if (eventType == QuestObjective.Type.Talk)
                {
                    TalkObjective talkObjective = (TalkObjective)quest.TriggerObjective;
                    if (talkObjective.NPC == eventName && QuestMeetsConditions(quest) && !IsQuestActiveOrCompleted(quest))
                    {
                        // Check that the player meets the quest conditions

                        ActiveQuests.Add(quest);
                        quest.UpdateQuest();
                        return;
                    }
                }
            }
        }

        // If the updated quest has no objectives then store it to be removed from the active quests list
        Quest questToRemove = null;

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
                            if (killObjective.CurrentAmount >= killObjective.Amount)
                            {
                                // Kill objective is complete
                                quest.CurrentObjective.Remove(objective);
                                break;
                            }
                        }
                    }
                    else if (eventType ==  QuestObjective.Type.Talk)
                    {
                        // Modify the talk objective
                        TalkObjective talkObjective = ((TalkObjective)objective); 
                        if (talkObjective.NPC == eventName)
                        {
                            // Complete the objective if the right NPC is talked to
                            quest.CurrentObjective.Remove(objective);
                            break;
                        }
                    }
                    else if (eventType == QuestObjective.Type.Item)
                    {
                        // Modify the item objective
                        ItemObjective itemObjective = ((ItemObjective)objective);
                        if (itemObjective.Item == eventName)
                        {
                            itemObjective.CurrentAmount++;
                            // If all the items are found then completed the objective
                            if (itemObjective.CurrentAmount >= itemObjective.Amount)
                            {
                                // Complete the objective if the right NPC is talked to
                                quest.CurrentObjective.Remove(objective);
                            }
                            break;
                        }
                    }
                } 
            }

            quest.UpdateQuest();

            UpdateNPCEntitesDialogues();
            if (quest.CurrentObjective.Count == 0)
            {
                // If there are no more objectives for this active quest then it is considered completed
                questToRemove = quest;

                // Give player item rewards
                foreach(string itemReward in quest.ItemRewards)
                {
                    string[] parsedItemReward = itemReward.Split(':');
                    GetComponent<Inventory>().AddItem($"Prefabs/{parsedItemReward[0]}", Int32.Parse(parsedItemReward[1]));
                }

                // Give player soul reward
                if(quest.SoulReward != 0)
                {
                    GetComponent<LootManager>().SoulCount += quest.SoulReward;
                } 
            }
        }

        if (questToRemove != null)
        {
            // If theres a quest to remove, remove it from the active list and place it in the completed quest
            CompletedQuests.Add(questToRemove);
            ActiveQuests.Remove(questToRemove);
            UpdateNPCEntitesDialogues();
        }
    }

    private bool IsQuestActiveOrCompleted(Quest quest)
    {
        // Check if the quest is either completed or active 

        // Check the active quests
        foreach(Quest activeQuest in ActiveQuests)
        {
            // Quest is in active quest list
            if (activeQuest.Title == quest.Title)
            {
                return true;
            }
        }

        // Check the completed quests
        foreach(Quest completedQuest in CompletedQuests)
        {
            // Quest is in completed quest list
            if (completedQuest.Title == quest.Title)
            {
                return true;
            }
        }

        // No quest found
        return false;
    }

    private bool QuestMeetsConditions(Quest quest)
    {
        if (quest.Conditions.Length == 0)
        {
            // No conditions
            return true;
        }

        bool questConditions = false;

        foreach(string condition in quest.Conditions)
        {
            // Check if the condition is a quest
            foreach(Quest completedQuest in CompletedQuests)
            {
                if (completedQuest.Title == condition)
                {
                    questConditions = true;
                }
            }

            // TODO: Add item conditions 
        }

        return questConditions;
    }

    public bool CheckQuestAndObjectiveStatus(string questTitle, string objectiveTitle)
    {
        switch(objectiveTitle)
        {
            case "":
                // Quest is currently active and no objective is needed to trigger this check
                foreach(Quest quest in ActiveQuests)
                {
                    if (quest.Title == questTitle)
                    {
                        return true;
                    }
                }
                break;
            case "COMPLETED":
                // Quest must be completed
                foreach(Quest quest in CompletedQuests)
                {
                    if (quest.Title == questTitle)
                    {
                        return true;
                    }
                }
                break;
            default:
                // quest must have an objective currently active to trigger this check
                foreach(Quest quest in ActiveQuests)
                {
                    if (quest.Title == questTitle)
                    {
                        foreach(QuestObjective objective in quest.CurrentObjective)
                        {
                            if (objective.Title == objectiveTitle)
                            {
                                return true;
                            }
                        }
                    }
                }
                break;
        }
        return false;
    }

    private void UpdateNPCEntitesDialogues()
    {
        foreach(GameObject npc in NPCEntites)
        {
            npc.GetComponent<INPC>().GetDialogueManager().TriggerEvent(this);
        }
    }

    void OnApplicationQuit()
    {
        Save();
    }
}
