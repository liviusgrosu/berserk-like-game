using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyEntityStatEffect : MonoBehaviour, IStatEffect
{
    public string StatName;
    public float Amount;
    public float Duration;
    private float _currentDuration;
    private EntityStats _entityStats;

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

            Destroy(this.gameObject);
        }
    }

    public void ProvideStats(EntityStats stats)
    {
        _entityStats = stats;
        _currentDuration = 0.0f;

        // Apply the effect
        ModifyStat(Amount);
    }

    private void ModifyStat(float amount)
    {
        switch(StatName)
        {
            case "Stamina Regeneration":
                _entityStats.CurrentStaminaRegeneration += amount;
                break;
        }
    }
}
