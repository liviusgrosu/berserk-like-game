
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStat
{
    public float Health;
    public float Stamina;
    // Passive stamina regeneration
    public float StaminaRegeneration;
    public EntityStat(float health, float stamina, float staminaRegeneration)
    {
        Health = health;
        Stamina = stamina;
        StaminaRegeneration = staminaRegeneration;
    }
}