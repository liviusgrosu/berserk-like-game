using UnityEngine;
using UnityEngine.UI;

class PlayerStatsUI : MonoBehaviour
{
    public RectTransform HealthBar;
    public Slider HealthBarSlider;

    public RectTransform StaminaBar;
    public Slider StaminaBarSlider;

    public EntityStats EntityStats;
    public Constants Constants;

    private float _originalHealthBarSize, _originalStaminaBarSize;
    private float _maxSliderSize;

    void Start()
    {
        _maxSliderSize = GetComponent<RectTransform>().sizeDelta.x;

        // Setup the base stats size
        _originalHealthBarSize = HealthBar.sizeDelta.x;
        _originalStaminaBarSize = StaminaBar.sizeDelta.x;

        // Health Bar
        HealthBarSlider.maxValue = EntityStats.Stats.Health;
        HealthBar.sizeDelta = new Vector2(_maxSliderSize * (EntityStats.Stats.Health / Constants.MaxPlayerLife), HealthBar.sizeDelta.y);
        HealthBarSlider.value = EntityStats.CurrentHealth;

        // Stamina Bar
        StaminaBarSlider.maxValue = EntityStats.Stats.Stamina;
        StaminaBar.sizeDelta = new Vector2(_maxSliderSize * (EntityStats.Stats.Stamina / Constants.MaxPlayerStamina), StaminaBar.sizeDelta.y);
        StaminaBarSlider.value = EntityStats.CurrentStamina;
    }

    void Update()
    {
        // Health bar update
        if (HealthBarSlider.maxValue != EntityStats.Stats.Health)
        {
            HealthBarSlider.maxValue = EntityStats.Stats.Health;
            HealthBar.sizeDelta = new Vector2(_maxSliderSize * (EntityStats.Stats.Health / Constants.MaxPlayerLife), HealthBar.sizeDelta.y);
        }

        // Stamina bar update
        if (StaminaBarSlider.maxValue != EntityStats.Stats.Stamina)
        {
            StaminaBarSlider.maxValue = EntityStats.Stats.Stamina;
            StaminaBar.sizeDelta = new Vector2(_maxSliderSize * (EntityStats.Stats.Stamina / Constants.MaxPlayerStamina), StaminaBar.sizeDelta.y);
        }

        HealthBarSlider.value = EntityStats.CurrentHealth;
        StaminaBarSlider.value = EntityStats.CurrentStamina;
    }
}