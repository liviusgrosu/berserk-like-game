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
    private Quest _activeQuest;
    public string TempCurrentQuestName;

    void Awake()
    {
        if (TempCurrentQuestName != null)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText("./Assets/JSON/quests.txt"));
            DataTable questDataTable = dataSet.Tables["Quests"];
            DataTable objectiveDataTable = dataSet.Tables["Objectives"];

            foreach(DataRow questRow in questDataTable.Rows)
            {
                Quest currentQuest = new Quest(
                                        (string)questRow["Title"], 
                                        (string)questRow["Description"],
                                        (string[])questRow["ItemRewards"], 
                                        (string)questRow["SoulReward"],
                                        (string)questRow["Status"]
                                        );
                Quests.Add(currentQuest);
            }
        }
    }
}
