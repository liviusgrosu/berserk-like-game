using UnityEngine;
using UnityEngine.UI;

class PlayerUI : MonoBehaviour
{
    public RectTransform HealthBar;
    public Slider HealthBarSlider;

    public RectTransform StaminaBar;
    public Slider StaminaBarSlider;

    private EntityStats EntityStats;

    private float _originalHealthBarSize, _originalStaminaBarSize;

    void Start()
    {
        EntityStats = GetComponent<EntityStats>();

        _originalHealthBarSize = HealthBar.sizeDelta.x;
        _originalStaminaBarSize = StaminaBar.sizeDelta.x;

        // Health Bar
        HealthBarSlider.maxValue = EntityStats.Health;
        HealthBar.sizeDelta = new Vector2(_originalHealthBarSize * EntityStats.Health, HealthBar.sizeDelta.y);
        HealthBarSlider.value = EntityStats.CurrentHealth;

        // Stamina Bar
        StaminaBarSlider.maxValue = EntityStats.Stamina;
        StaminaBar.sizeDelta = new Vector2(_originalStaminaBarSize * EntityStats.Stamina, StaminaBar.sizeDelta.y);
        StaminaBarSlider.value = EntityStats.CurrentStamina;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EntityStats.CurrentHealth--;
            HealthBarSlider.value = EntityStats.CurrentHealth;
        }

        // Health Bar Update
        if (HealthBarSlider.maxValue != EntityStats.Health)
        {
            HealthBarSlider.maxValue = EntityStats.Health;
            HealthBar.sizeDelta = new Vector2(_originalHealthBarSize * EntityStats.Health, HealthBar.sizeDelta.y);
        }

        // Stamina Bar Update
        if (StaminaBarSlider.maxValue != EntityStats.Stamina)
        {
            StaminaBarSlider.maxValue = EntityStats.Stamina;
            StaminaBar.sizeDelta = new Vector2(_originalStaminaBarSize * EntityStats.Stamina, StaminaBar.sizeDelta.y);
        }
    }
}