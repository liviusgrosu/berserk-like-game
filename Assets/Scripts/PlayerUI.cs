using UnityEngine;
using UnityEngine.UI;

class PlayerUI : MonoBehaviour
{
    public RectTransform HealthBar;
    public Slider HealthBarSlider;

    public RectTransform StaminaBar;
    public Slider StaminaBarSlider;

    private EntityStats EntityStats;

    void Start()
    {
        EntityStats = GetComponent<EntityStats>();

        // Health Bar
        HealthBarSlider.maxValue = EntityStats.Health;
        HealthBar.sizeDelta = new Vector2(HealthBar.sizeDelta.x * EntityStats.Health, HealthBar.sizeDelta.y);
        HealthBarSlider.value = EntityStats.CurrentHealth;

        // Stamina Bar
        StaminaBarSlider.maxValue = EntityStats.Health;
        StaminaBar.sizeDelta = new Vector2(StaminaBar.sizeDelta.x * EntityStats.Stamina, StaminaBar.sizeDelta.y);
        StaminaBarSlider.value = EntityStats.CurrentHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EntityStats.CurrentHealth--;
            HealthBarSlider.value = EntityStats.CurrentHealth;
        }
    }
}