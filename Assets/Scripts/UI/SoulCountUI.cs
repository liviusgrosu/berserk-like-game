using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulCountUI : MonoBehaviour
{
    public Text SoulCount;
    public LootManager LootManager;
    public float IncrementMultiplier = 100.0f;
    // UI soul count is a float so we can fix the increment with time.deltatime
    private float _uiSoulCount;
    private bool _isCounting;

    void Start()
    {
        // Display the soul count
        _uiSoulCount = LootManager.SoulCount;
        SoulCount.text = _uiSoulCount.ToString();
    }

    void Update()
    {
        // If souls are aquired start counting up
        if (_uiSoulCount != LootManager.SoulCount)
        {
            _isCounting = true;
        }

        // Count the UI soul count
        if (_isCounting)
        {
            _uiSoulCount += Time.deltaTime * IncrementMultiplier;
            if ((int)_uiSoulCount >= LootManager.SoulCount)
            {
                _isCounting = false;
                _uiSoulCount = LootManager.SoulCount;
            }
            SoulCount.text = ((int)_uiSoulCount).ToString();
        }
    }
}
