using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Effects;
    private EntityStats _playerStats;

    void Awake()
    {
        _playerStats = GetComponent<EntityStats>();
    }

    void Update()
    {
        for(int idx = 0; idx < Effects.Count; idx++)
        {
            EntityEffects effect = Effects[idx].GetComponent<EntityEffects>();
            // Add to the current stats
            _playerStats.AddCurrentStats(effect);
            // Decrement the duration
            effect.Duration -= Time.deltaTime;
            // Remove it once the duration is 0
            if (effect.Duration <= 0)
            {
                Effects.RemoveAt(idx);
            }
        }
    }
}