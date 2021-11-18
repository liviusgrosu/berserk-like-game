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

    public bool Talking;
    private int _dialogueIdx;

    public DialogueManager(string file)
    {
        AllRegularDialogue = new List<Dialogue>();
        AllSpecialDialogue = new List<Dialogue>();

        FinishedRegularDialogue = new List<Dialogue>();
        FinishedSpecialDialogue = new List<Dialogue>();

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

    public string GetNextLine()
    {
        if (CurrentSpecialDialogue != null)
        {
            // If special dialogue exists then get those lines instead
            string lineToReturn = GetNextCurrentLine(CurrentSpecialDialogue);
            if (lineToReturn == "")
            {
                // Remove the special dialogue once they've gone through
                CurrentSpecialDialogue = null;
            } 
            return lineToReturn;
        }
        else
        {
            // Return regular dialogue lines
            string lineToReturn = GetNextCurrentLine(CurrentRegularDialogue);
            return lineToReturn;
        }
    }

    private string GetNextCurrentLine(Dialogue currentDialogue)
    {
        if (_dialogueIdx >= currentDialogue.Lines.Length)
        {
            // No more lines available
            _dialogueIdx = 0;
            return "";
        }

        // Return the next line
        string lineToReturn = currentDialogue.Lines[_dialogueIdx]; 
        _dialogueIdx++;
        return lineToReturn;
    }

    public void TriggerEvent(QuestManager questManager)
    {
        // Check all regular dialogue
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

            if (dialogueMeetsConditions && !FinishedRegularDialogue.Contains(regularDialogue))
            {
                // Add new regular dialogue if all conditions are meet
                CurrentRegularDialogue = regularDialogue;
                FinishedRegularDialogue.Add(regularDialogue);
            }
        }
        
        // Check all special dialogue

        foreach(Dialogue specialDialogue in AllSpecialDialogue)
        {
            bool dialogueMeetsConditions = true;
            // Go through all the objectives 
            foreach(Dialogue.QuestCondition questCondition in specialDialogue.QuestConditions)
            {
                if (!questManager.CheckQuestAndObjectiveStatus(questCondition.Title, questCondition.Objective))
                {
                    dialogueMeetsConditions = false;
                }
            }

            if (dialogueMeetsConditions && !FinishedSpecialDialogue.Contains(specialDialogue))
            {
                // Add new regular dialogue if all conditions are meet
                CurrentSpecialDialogue = specialDialogue;
                FinishedSpecialDialogue.Add(specialDialogue);
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
                    // Assign new regular dialogue
                    CurrentRegularDialogue = regularDialogue;
                    // Don't use this dialogue after its used
                    FinishedRegularDialogue.Add(regularDialogue);
                    break;
                }
            }

            foreach(Dialogue specialDialogue in AllSpecialDialogue)
            {
                if (specialDialogue.QuestConditions.Count == 0)
                {
                    // Assign new special dialogue
                    CurrentSpecialDialogue = specialDialogue;
                    // Don't use this dialogue after its used
                    FinishedSpecialDialogue.Add(specialDialogue);
                    break;
                }
            }
        }
    }

    public void Save()
    {

    }
}
