using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    private Equipment _equipmentData;
    // TODO: add access to buffs to get attack damage buff
    private EntityAttacking EntityAttackState;

    void Start()
    {
        // TODO: change to access the current entities stats not the players
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
            float damage = ((WeaponStats)_equipmentData.Stats).Damage;

            // Assign damage to the entity
            collider.GetComponent<EntityStats>().ReduceHealth(damage);
        }
    }

    public void AssignData(Equipment equipmentData)
    {
        // Give this script the current stats
        _equipmentData = equipmentData;
    }
}
