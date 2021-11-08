using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstantEntityStatEffect : MonoBehaviour, IStatEffect
{
    public string StatName;
    public float Amount;
    public Sprite Icon;
    private EntityStats _entityStats;
    private Action<GameObject> _destroyCallback;

    void Update()
    {
        if (_entityStats == null)
        {
            return;
        }

        // Add the change to the players entites stat
        switch(StatName)
        {
            case "Health":
                _entityStats.CurrentHealth += Amount;
                break;
            case "Stamina":
                _entityStats.CurrentStamina += Amount;
                break;
        }

        // Remove the effect from the effect list
        _destroyCallback(this.gameObject);
        // Remove the effect once its done
        Destroy(this.gameObject);
    }

    public void ProvideStats(EntityStats stats, Action<GameObject> destroyCallback)
    {
        // Recieve the stats
        _entityStats = stats;
        _destroyCallback = destroyCallback;
    }

    public Sprite GetIcon()
    {
        // Get the icon
        return Icon;
    }
}
