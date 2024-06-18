using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSlider : EaseSlider
{
    [SerializeField] private Health health;
 
    private void Awake()
    {
        if (health != null)
        {
            health.onHealthDamage += Health_onHealthDamage;
            Initialize(health.GetMaxHealth());
        }

    }

    private void Health_onHealthDamage(float amount, bool isCritical)
    {
        SetCurrentValue(health.GetCurrentHealth());
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.onHealthDamage -= Health_onHealthDamage;
        }
    }
}
