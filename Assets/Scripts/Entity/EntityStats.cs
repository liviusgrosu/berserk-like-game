using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [Header("Stats")]
    // Stats
    public float Health;
    public float Stamina;
    // Passive stamina regeneration
    public float StaminaRegeneration;
    public float Defence;
    public float AttackSpeed;
    // Current stats
    [HideInInspector] public float CurrentHealth, CurrentStamina, CurrentAttackSpeed;
    // Used only for buffs
    [HideInInspector] public float CurrentStaminaRegeneration = 0f;
    // List of upgrade ids from the skill tree
    [HideInInspector] public List<SkillTreeNode> CurrentSkills;
    public float ActivateStaminaRegenTime = 0.7f;
    private float _currentStaminaRegenDeactivationTime = 0f;
    private bool _staminaProductionStopped;

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
        // Regenerate stamina
        if (!_staminaProductionStopped)
        {
            CurrentStamina = Mathf.Clamp(CurrentStamina + (CurrentStaminaRegeneration + StaminaRegeneration) * Time.deltaTime, 0, Stamina);
        }
        else
        {
            // TODO: add a stat that affects the time it takes for stamina regenation to activate
            // Wait for stamina regeneration to activate
            _currentStaminaRegenDeactivationTime += Time.deltaTime;
            if (_currentStaminaRegenDeactivationTime >= ActivateStaminaRegenTime)
            {
                _staminaProductionStopped = false;
            }
        }
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
        // Add skill upgrade and increment the respective stat
        CurrentSkills.Add(skillNode);
        switch (skillNode.SkillName)
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
        // TODO: add saving feature
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

    public void ReduceStamina(float amount)
    {
        // Reason for having function like is so we can introduce stamina reduction stats
        CurrentStamina -= amount;
        // Stop stamina regeneration
        _staminaProductionStopped = true;
        _currentStaminaRegenDeactivationTime = 0.0f;
    }
}
