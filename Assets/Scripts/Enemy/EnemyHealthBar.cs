using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider HealthBarSlider;
    public EntityStats EntityStats;

    void Start()
    {
        // Init the health bar
        HealthBarSlider.maxValue = EntityStats.Stats.Health;
        HealthBarSlider.value = EntityStats.CurrentHealth;
    }

    void Update()
    {
        // Update the health bar
        HealthBarSlider.value = EntityStats.CurrentHealth;
    }
}
