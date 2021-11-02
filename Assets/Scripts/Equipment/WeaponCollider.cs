using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    // TODO: add access to buffs to get attack damage buff
    private EntityCombat EntityAttackState;

    void Start()
    {
        // TODO: we might need to change this depending on how the gameobject is structured
        EntityAttackState = transform.root.GetComponent<EntityCombat>();
    }

    void OnTriggerEnter(Collider collider)
    {
        // Don't deal damage unless attacking
        if (!EntityAttackState.IsAttacking())
        {
            return;
        }
        
        if (collider.tag == "Damagable" && collider.name != transform.root.name)
        {
            // TODO BEGIN: 
            // attack triggers multiple times 
            // Add key triggers in animations to allow window of damage dealt

            // Calculate damage
            float damage = GetComponent<WeaponEquipment>().Stats.Damage;

            // Assign damage to the entity
            collider.GetComponent<IEntity>().RecieveHit(damage);
        }
    }
}
