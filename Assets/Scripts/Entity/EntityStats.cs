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

    // Current
    public int CurrentHealth;
    public int CurrentStamina;

    void Start()
    {
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
}
