using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    [HideInInspector]
    private EquipmentStats EquipmentStats;
    // TODO: add access to buffs to get attack damage buff
    private EntityAttacking EntityAttackState;

    void Start()
    {
        EntityAttackState = GameObject.Find("Player").GetComponent<EntityAttacking>();
    }

    void OnTriggerEnter(Collider collider)
    {
        // Don't deal damage unless attacking
        if (!EntityAttackState.IsAttacking())
        {
            return;
        }
        
        if (collider.tag == "Damagable")
        {
            // TODO BEGIN: 
            // attack triggers multiple times 
            // Add key triggers in animations to allow window of damage dealt

            // Calculate damage
            float damage = ((WeaponStats)EquipmentStats).Damage;

            // Assign damage to the entity
            collider.GetComponent<EntityStats>().ReduceHealth(damage);
            Debug.Log(collider.GetComponent<EntityStats>().CurrentHealth);
        }
    }

    public void AssignStats(EquipmentStats equipmentStats)
    {
        EquipmentStats = equipmentStats;
    }
}
