using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> Effects;
    public Transform BuffParent;
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
                GameObject currentEffect = Effects[idx];
                Effects.RemoveAt(idx);
                Destroy(currentEffect);
            }
        }
    }

    public void AddBuff(GameObject effect)
    {
        // Instantiate the effect and add it to the list
        GameObject newEffect = Instantiate(effect, BuffParent.position, Quaternion.identity);
        newEffect.transform.parent = BuffParent;
        Effects.Add(newEffect);
    }
}