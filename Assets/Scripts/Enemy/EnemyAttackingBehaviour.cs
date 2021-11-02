using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingBehaviour : MonoBehaviour, IEntity
{
    public float AttackingCooldown = 2.0f;
    private float _currentAttackingCooldown;
    private EntityCombat _EntityCombat;
    private EnemyStateBehaviour _stateBehaviour;
    private EnemyEquipments _equipments;
    private EntityStats _entityStats;

    void Start()
    {
        _EntityCombat = GetComponent<EntityCombat>();
        _stateBehaviour = GetComponent<EnemyStateBehaviour>();
        _equipments = GetComponent<EnemyEquipments>();
        _entityStats = GetComponent<EntityStats>();
        
        // Attack right away
        _currentAttackingCooldown = AttackingCooldown;
    }

    void Update()
    {
        if (_stateBehaviour.CurrentState == EnemyStateBehaviour.States.Attack)
        {
            // Once entity is within attacking range start attacking on an interval
            _currentAttackingCooldown += Time.deltaTime;
            // Set a cooldown between attacks
            if (_currentAttackingCooldown >= AttackingCooldown)
            {
                _EntityCombat.TriggerAttack(_stateBehaviour.Player.position, _equipments.GetCurrentEquipmentStats().AttackSpeed);
                _currentAttackingCooldown = 0.0f;
            }
        }
        else
        {
            // Next time target gets within attacking range, attack right away
            _currentAttackingCooldown = AttackingCooldown;
        }
    }

    public void RecieveHit(float damage)
    {
        // Reduce entity health
        _entityStats.ReduceHealth(damage);
    }
}
