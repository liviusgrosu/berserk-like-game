using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // Use stats that are filled in by editor
    public bool UseEditorStats;
    public EntityStat Stats;
    // Current stats
    [HideInInspector] public float CurrentHealth, CurrentStamina, CurrentAttackSpeed;
    // Used only for buffs
    [HideInInspector] public float CurrentStaminaRegeneration = 0.0f, CurrentHealthRegeneration = 0.0f;
    // List of upgrade ids from the skill tree
    [HideInInspector] public List<int> CurrentSkillsId;
    public float ActivateStaminaRegenTime = 0.7f;
    private float _currentStaminaRegenDeactivationTime = 0.0f;
    private bool _staminaProductionStopped;
    [HideInInspector] public bool EnableIFrames;

    void Awake()
    {
        if (!UseEditorStats)
        {
            Load();
        }

        CurrentHealth = Stats.Health;
        CurrentStamina = Stats.Stamina;
    }

    void Update()
    {
        // Clamp & regenerate the stats
        CurrentHealth = Mathf.Clamp(CurrentHealth + (CurrentHealthRegeneration + Stats.HealthRegeneration) * Time.deltaTime, 0, Stats.Health);

        // Regenerate stamina
        if (!_staminaProductionStopped)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + (CurrentStaminaRegeneration + Stats.StaminaRegeneration) * Time.deltaTime, 0, Stats.Stamina);
        }
        else
        {
            // Wait for stamina regeneration to activate
            _currentStaminaRegenDeactivationTime += Time.deltaTime;
            if (_currentStaminaRegenDeactivationTime >= ActivateStaminaRegenTime)
            {
                _staminaProductionStopped = false;
            }
        }
    }

    public void AddUpgrade(SkillTreeNode skillNode)
    {
        // Add skill upgrade and increment the respective stat
        CurrentSkillsId.Add(skillNode.ID);
        switch (skillNode.SkillName)
        {
            case "health":
                Stats.Health += skillNode.Amount;
                break;
            case "stamina":
                Stats.Stamina += skillNode.Amount;
                break;
            default:
                break;
        }
    }

    public bool CheckIfUpgradeUnlocked(SkillTreeNode skillNode)
    {
        return CurrentSkillsId.Contains(skillNode.ID);
    }

    public void ReduceStamina(float amount)
    {
        // Reason for having function like is so we can introduce stamina reduction stats
        CurrentStamina -= amount;
        // Stop stamina regeneration
        _staminaProductionStopped = true;
        _currentStaminaRegenDeactivationTime = 0.0f;
    }

    public void ReduceHealth(float amount)
    {
        // TODO: change this so that weapons can only ignore i frames
        if (!EnableIFrames)
        {
            CurrentHealth -= amount;  
        }
    }

    private void Save()
    {
        // --- Stats ---
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create($"{Application.persistentDataPath}/{gameObject.name}.entityStats");
        // Serialize the entities stats and save it to its respective file
        bf.Serialize(file, Stats);
        file.Close();
        // --- Skills ---
        file = File.Create($"{Application.persistentDataPath}/{gameObject.name}.entitySkills");
        bf.Serialize(file, CurrentSkillsId);
        file.Close();
    }

    public void Load()
    {
        // --- Stats ---
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/{gameObject.name}.entityStats"))
        {
            // Open the file
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{gameObject.name}.entityStats", FileMode.Open);
            Stats = (EntityStat)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            Stats = new EntityStat(10.0f, 10.0f, 0.0f, 10.0f);
        }
        // --- Skills ---
        // Check if the file exists
        if (File.Exists($"{Application.persistentDataPath}/{gameObject.name}.entitySkills"))
        {
            // Load the skill IDs if the file exists
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open($"{Application.persistentDataPath}/{gameObject.name}.entitySkills", FileMode.Open);
            CurrentSkillsId = (List<int>)bf.Deserialize(file);   
        }
        else 
        {
            // Create a new skill IDs list
            CurrentSkillsId = new List<int>();
        }
    }

    void OnApplicationQuit()
    {
        // Don't save if the editor stats are being used
        if (!UseEditorStats)
        {
            Save();
        }
    }
}
