using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyEntityStatEffect : MonoBehaviour, IStatEffect
{
    public string StatName;
    public float Amount;
    public Sprite Icon;
    public float Duration;
    private float _currentDuration;
    private EntityStats _entityStats;
    private Action<GameObject> _destroyCallback;


    void Update()
    {
        if (_entityStats == null)
        {
            return;
        }

        _currentDuration += Time.deltaTime;
        if (_currentDuration >= Duration)
        {
            // Revert the effect
            ModifyStat(-Amount);
            // Remove the effect from the effect list
            _destroyCallback(this.gameObject);
            // Destroy the effect once its done
            Destroy(this.gameObject);
        }
    }

    public void ProvideStats(EntityStats stats, Action<GameObject> destroyCallback)
    {
        _entityStats = stats;
        _destroyCallback = destroyCallback;

        _currentDuration = 0.0f;

        // Apply the effect
        ModifyStat(Amount);
    }

    private void ModifyStat(float amount)
    {
        // Add the change to the players entites stat
        switch(StatName)
        {
            case "Health Regeneration":
                _entityStats.CurrentHealthRegeneration += amount;
                break;
            case "Stamina Regeneration":
                _entityStats.CurrentStaminaRegeneration += amount;
                break;
        }
    }

    public Sprite GetIcon()
    {
        return Icon;
    }
}
