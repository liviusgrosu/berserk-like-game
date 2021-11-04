
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityStat
{
    public float Health;
    public float Stamina;
    // Passive stamina regeneration
    public float StaminaRegeneration;
    public float Defence;
    public EntityStat(float health, float stamina, float staminaRegeneration, float defence)
    {
        Health = health;
        Stamina = stamina;
        StaminaRegeneration = staminaRegeneration;
        Defence = defence;
    }
}