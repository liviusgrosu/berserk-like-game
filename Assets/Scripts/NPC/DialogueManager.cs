using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

[System.Serializable]
public class DialogueManager
{
    private List<Dialogue> AllRegularDialogue, AllSpecialDialogue;
    private List<Dialogue> FinishedRegularDialogue, FinishedSpecialDialogue;

    private Dialogue CurrentRegularDialogue, CurrentSpecialDialogue; 

    public DialogueManager(string file)
    {
        AllRegularDialogue = new List<Dialogue>();
        AllSpecialDialogue = new List<Dialogue>();

        DataSet dataset = JsonConvert.DeserializeObject<DataSet>(File.ReadAllText($"./Assets/JSON/{file}.json"));
        DataTable regularDataTable = dataset.Tables["Reguluar"];
        DataTable specialDataTable = dataset.Tables["Special"];

        foreach(DataRow dialogueRow in regularDataTable.Rows)
        {
            Dialogue dialogue = new Dialogue((string[])dialogueRow["Lines"]);

            string[] dialogueConditions = (string[])dialogueRow["Conditions"];
            foreach(string condition in dialogueConditions)
            {
                string[] conditionElements = condition.Split(':');
                switch(conditionElements[0])
                {
                    case "Quest":
                        dialogue.QuestConditions.Add(new Dialogue.QuestCondtion(conditionElements[1], conditionElements[2]));
                        break;
                    case "Item":
                        dialogue.ItemConditions.Add(new Dialogue.ItemCondition(conditionElements[1], Int32.Parse(conditionElements[2])));
                        break;
                    default:
                        break;
                }
            }

            AllRegularDialogue.Add(dialogue);
        }
    }

    public void Load()
    {

    }

    public void Save()
    {

    }
}
