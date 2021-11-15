using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
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

        AddDialogue(regularDataTable, AllRegularDialogue);
        AddDialogue(specialDataTable, AllSpecialDialogue);

        Load();
    }

    private void AddDialogue(DataTable dataTable, List<Dialogue> dialogueCollection)
    {
        foreach(DataRow dialogueRow in dataTable.Rows)
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

            dialogueCollection.Add(dialogue);
        }
    }

    public void Talk()
    {
        if (CurrentSpecialDialogue != null)
        {
            foreach(string line in CurrentSpecialDialogue.Lines)
            {
                Debug.Log(line);
            }
            CurrentSpecialDialogue = null;
        }
        else
        {
            foreach(string line in CurrentRegularDialogue.Lines)
            {
                Debug.Log(line);
            }
        }
    }

    public void TriggerEvent()
    {

    }

    public void Load()
    {
        if (File.Exists($"{Application.persistentDataPath}/Dialogue/Blacksmith.currentRegular"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            // Load the current regular dialogue
            FileStream file = File.Open($"{Application.persistentDataPath}/Dialogue/Blacksmith.currentRegular", FileMode.Open);
            CurrentRegularDialogue = (Dialogue)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            // Use dialogue that has no conditions
            foreach(Dialogue regularDialogue in AllRegularDialogue)
            {
                if (!regularDialogue.ConditionsExist())
                {
                    CurrentRegularDialogue = regularDialogue;
                    break;
                }
            }

            foreach(Dialogue specialDialogue in AllSpecialDialogue)
            {
                if (!specialDialogue.ConditionsExist())
                {
                    CurrentSpecialDialogue = specialDialogue;
                    break;
                }
            }
        }
    }

    public void Save()
    {

    }
}
