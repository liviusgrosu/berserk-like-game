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
                dialogue.QuestConditions.Add(new Dialogue.QuestCondition(conditionElements[0], conditionElements[1]));
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

    public void TriggerEvent(QuestManager questManager)
    {
        foreach(Dialogue regularDialogue in AllRegularDialogue)
        {
            bool dialogueMeetsConditions = true;
            // Go through all the objectives 
            foreach(Dialogue.QuestCondition questCondition in regularDialogue.QuestConditions)
            {
                if (!questManager.CheckQuestAndObjectiveStatus(questCondition.Title, questCondition.Objective))
                {
                    dialogueMeetsConditions = false;
                }
            }

            if (dialogueMeetsConditions)
            {
                // Add new regular dialogue if all conditions are meet
                CurrentRegularDialogue = regularDialogue;
            }
        }
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
                if (regularDialogue.QuestConditions.Count == 0)
                {
                    CurrentRegularDialogue = regularDialogue;
                    break;
                }
            }

            foreach(Dialogue specialDialogue in AllSpecialDialogue)
            {
                if (specialDialogue.QuestConditions.Count == 0)
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
