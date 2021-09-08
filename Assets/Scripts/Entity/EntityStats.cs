using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    // Stats
    public int Health;
    public int Stamina;
    public int StaminaRegeneration;
    public int Defence;
    public int AttackSpeed;

    [HideInInspector]
    // Current
    public int CurrentHealth, CurrentStamina;

    // List of upgrade ids from the skill tree
    private List<int> UpgradeIDs;

    void Awake()
    {
        UpgradeIDs = new List<int>();
        UpgradeIDs.Add(1);

        CurrentHealth = Health;
        CurrentStamina = Stamina;
    }

    void Update()
    {
        CurrentStamina = Mathf.Clamp(CurrentStamina + StaminaRegeneration, 0, Stamina);
    }

    void SaveStatsToFile()
    {
        // TODO: save stats to a file
    }

    void LoadStatsFromFile()
    {
        // TODO: load stats from a file
    }

    public void StoreUpgrade(int id)
    {
        UpgradeIDs.Add(id);
    }

    public bool CheckIfUpgradeUnlocked(int id)
    {
        return UpgradeIDs.Contains(id);
    }
}
