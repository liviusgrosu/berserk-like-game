
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStat
{
    public float Health;
    public float Stamina;
    public float HealthRegeneration;    
    public float StaminaRegeneration;
    public EntityStat(float health, float stamina, float healthRegeneration, float staminaRegeneration)
    {
        Health = health;
        Stamina = stamina;
        HealthRegeneration = healthRegeneration;
        StaminaRegeneration = staminaRegeneration;
    }
}