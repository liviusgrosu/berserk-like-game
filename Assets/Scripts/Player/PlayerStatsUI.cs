using UnityEngine;
using UnityEngine.UI;

class PlayerStatsUI : MonoBehaviour
{
    public RectTransform HealthBar;
    public Slider HealthBarSlider;

    public RectTransform StaminaBar;
    public Slider StaminaBarSlider;

    public EntityStats EntityStats;

    private float _originalHealthBarSize, _originalStaminaBarSize;

    void Start()
    {
        _originalHealthBarSize = HealthBar.sizeDelta.x;
        _originalStaminaBarSize = StaminaBar.sizeDelta.x;

        // Health Bar
        HealthBarSlider.maxValue = EntityStats.Stats.Health;
        HealthBar.sizeDelta = new Vector2(_originalHealthBarSize * EntityStats.Stats.Health, HealthBar.sizeDelta.y);
        HealthBarSlider.value = EntityStats.CurrentHealth;

        // Stamina Bar
        StaminaBarSlider.maxValue = EntityStats.Stats.Stamina;
        StaminaBar.sizeDelta = new Vector2(_originalStaminaBarSize * EntityStats.Stats.Stamina, StaminaBar.sizeDelta.y);
        StaminaBarSlider.value = EntityStats.CurrentStamina;
    }

    void Update()
    {
        // Health bar update
        if (HealthBarSlider.maxValue != EntityStats.Stats.Health)
        {
            HealthBarSlider.maxValue = EntityStats.Stats.Health;
            HealthBar.sizeDelta = new Vector2(_originalHealthBarSize * EntityStats.Stats.Health, HealthBar.sizeDelta.y);
        }

        // Stamina bar update
        if (StaminaBarSlider.maxValue != EntityStats.Stats.Stamina)
        {
            StaminaBarSlider.maxValue = EntityStats.Stats.Stamina;
            StaminaBar.sizeDelta = new Vector2(_originalStaminaBarSize * EntityStats.Stats.Stamina, StaminaBar.sizeDelta.y);
        }

        HealthBarSlider.value = EntityStats.CurrentHealth;
        StaminaBarSlider.value = EntityStats.CurrentStamina;
    }
}