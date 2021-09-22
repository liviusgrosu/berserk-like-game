using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // Stats
    public float Health;
    public float Stamina;
    public float StaminaRegeneration;
    public float Defence;
    public float AttackSpeed;

    [HideInInspector]
    // Current
    public float CurrentHealth, CurrentStamina, CurrentStaminaRegeneration, CurrentAttackSpeed;

    // List of upgrade ids from the skill tree
    public List<SkillTreeNode> CurrentSkills;

    void Awake()
    {
        if (CurrentSkills == null)
        {
            CurrentSkills = new List<SkillTreeNode>();
        }

        CurrentHealth = 2;
        CurrentStamina = 2;
    }

    void Update()
    {
        // CurrentStamina = Mathf.Clamp(CurrentStamina + StaminaRegeneration, 0, Stamina);
    }

    void SaveStatsToFile()
    {
        // TODO: save stats to a file
    }

    void LoadStatsFromFile()
    {
        // TODO: load stats from a file
    }

    public void AddUpgrade(SkillTreeNode skillNode)
    {
        CurrentSkills.Add(skillNode);
        string skillName = skillNode.Skill;
        switch (skillName)
        {
            case "health":
                Health += skillNode.Amount;
                break;
            case "stamina":
                Stamina += skillNode.Amount;
                break;
            case "attack speed":
                AttackSpeed += skillNode.Amount;
                break;
            default:
                break;
        }
    }

    public void AddCurrentStats(EntityEffects effect)
    {
        string effectName = effect.Name;
        switch (effectName)
        {
            case "health":
                CurrentHealth += effect.Rate;
                CurrentHealth = Mathf.Clamp(CurrentHealth, 0, Health);
                break;
            case "stamina":
                CurrentStamina += effect.Rate;
                CurrentStamina = Mathf.Clamp(CurrentStamina, 0, Stamina);
                break;
            case "stamina regeneration":
                CurrentStaminaRegeneration += effect.Rate;
                CurrentStaminaRegeneration = Mathf.Clamp(CurrentStaminaRegeneration, 0, StaminaRegeneration);
                break;
            case "attack speed":
                CurrentAttackSpeed += effect.Rate;
                CurrentAttackSpeed = Mathf.Clamp(CurrentAttackSpeed, 0, AttackSpeed);
                break;
            default:
                break;
        }
    }

    public bool CheckIfUpgradeUnlocked(SkillTreeNode skillNode)
    {
        return CurrentSkills.Contains(skillNode);
    }
}
