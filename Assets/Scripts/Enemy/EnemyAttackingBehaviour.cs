using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingBehaviour : MonoBehaviour
{
    public float AttackingCooldown = 2.0f;
    private float _currentAttackingCooldown;
    private EntityAttacking _entityAttacking;
    private EnemyStateBehaviour _stateBehaviour;

    void Awake()
    {
        _entityAttacking = GetComponent<EntityAttacking>();
        _stateBehaviour = GetComponent<EnemyStateBehaviour>();
        
        // Attack right away
        _currentAttackingCooldown = AttackingCooldown;
    }

    void Update()
    {
        if (_stateBehaviour.CurrentState == EnemyStateBehaviour.States.Attack)
        {
            // Once entity is within attacking range start attacking on an interval
            _currentAttackingCooldown += Time.deltaTime;
            if (_currentAttackingCooldown >= AttackingCooldown)
            {
                _entityAttacking.TriggerAttack(_stateBehaviour.Player.position);
                _currentAttackingCooldown = 0.0f;
            }
        }
        else
        {
            // Next time target gets within attacking range, attack right away
            _currentAttackingCooldown = AttackingCooldown;
        }
    }
}
