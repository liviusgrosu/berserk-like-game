using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChangePlayerStats : MonoBehaviour
{
    public EntityStats PlayerStats;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            PlayerStats.CurrentStamina -= 10.0f;
        }
    }
}
