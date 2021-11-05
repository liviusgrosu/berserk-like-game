using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantEntityStatEffect : MonoBehaviour, IStatEffect
{
    public string StatName;
    public float Amount;
    private EntityStats _entityStats;

    void Update()
    {
        if (_entityStats == null)
        {
            return;
        }

        switch(StatName)
        {
            case "Health":
                _entityStats.CurrentHealth += Amount;
                break;
            case "Stamina":
                _entityStats.CurrentStamina += Amount;
                break;
        }

        Destroy(this.gameObject);
    }

    public void ProvideStats(EntityStats stats)
    {
        _entityStats = stats;
    }
}
