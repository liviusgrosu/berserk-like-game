using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackingBehaviour : MonoBehaviour
{
    private EntityAttacking _entityAttacking;

    void Awake()
    {
        _entityAttacking = GetComponent<EntityAttacking>();
    }
}
