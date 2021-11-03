using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoulCountUI : MonoBehaviour
{
    public Text SoulCount;
    public Text AquiredSoulCount;
    public LootManager LootManager;
    public float IncrementMultiplier = 100.0f;
    // UI soul count is a float so we can fix the increment with time.deltatime
    private float _uiSoulCount;
    private int _uiAquiredSoulCount, _uiStartingSoulCount;
    private bool _isCounting;
    private bool _fadeOutAquiredSoulText;
    private float _fadeTime;
    public float FadeDuration = 1.0f;
    public float FadeMultiplier = 1.0f;
    void Start()
    {
        // Display the soul count
        _uiSoulCount = LootManager.SoulCount;
        SoulCount.text = _uiSoulCount.ToString();
        AquiredSoulCount.color =  new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    void Update()
    {
        // If souls are aquired start counting up
        if (_uiSoulCount != LootManager.SoulCount && !_isCounting)
        {
            _isCounting = true;
            _uiStartingSoulCount = (int)_uiSoulCount;
            // If it happens to be already fading out then set back the colour to opaque
            _fadeOutAquiredSoulText = false;
            AquiredSoulCount.color =  new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        // Count the UI soul count
        if (_isCounting)
        {
            _uiSoulCount += Time.deltaTime * IncrementMultiplier;
            if ((int)_uiSoulCount >= LootManager.SoulCount)
            {
                // Start fading out the aquired souls text
                _isCounting = false;
                _uiSoulCount = LootManager.SoulCount;
                _fadeOutAquiredSoulText = true;
                _fadeTime = FadeDuration;
            }
            SoulCount.text = ((int)_uiSoulCount).ToString();
            AquiredSoulCount.text = $"+{LootManager.SoulCount - _uiStartingSoulCount}";
        }

        if (_fadeOutAquiredSoulText)
        {
            // Fade out the aquired souls text via the alpha channel of its colour
            _fadeTime -= Time.deltaTime * FadeMultiplier;
            AquiredSoulCount.color = new Color(1.0f, 1.0f, 1.0f, _fadeTime / FadeDuration);
            
            if (_fadeTime <= 0.0f)
            {
                _fadeOutAquiredSoulText = false;
            }
        }
    }

    void OnDisable()
    {
        // Finish the counting process
        _isCounting = false;
        _uiSoulCount = LootManager.SoulCount;
        SoulCount.text = ((int)_uiSoulCount).ToString();

        // Finish the fading process
        AquiredSoulCount.color =  new Color(1.0f, 1.0f, 1.0f, 0.0f);
        _fadeOutAquiredSoulText = false;
    }
}
