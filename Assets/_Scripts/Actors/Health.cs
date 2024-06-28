using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Health : MonoBehaviour
{
    public delegate void HealthZeroHandler(GameObject killer);
    public event HealthZeroHandler onHealthZero;

    //Event used to flag if the actor's current health has changed, and by how much
    public delegate void HealthChangeHandler(float amount, bool isCritical);
    public event HealthChangeHandler onHealthDamage;
    public event HealthChangeHandler onHealthHeal;

    public delegate void DamageDraw(Vector3 position, float amount, bool isCritical);
    public static event DamageDraw OnDamageDraw;


    [SerializeField] private float maxHealth;

    [SerializeField] private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(GameObject source, float damage, bool isCritical)
    {
        if(damage > 0)
        {
            currentHealth -= damage;

            //Flag that this game object has taken damage
            onHealthDamage?.Invoke(damage, isCritical);
            OnDamageDraw?.Invoke(transform.position, damage, isCritical);


            // Check if the enemy has been killed
            if (currentHealth <= 0)
            {
                onHealthZero?.Invoke(source); //raise a death flag
            }
        } else if(damage < 0)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"Tried to damage {name} but the value was negative ({damage})! Are you sure this is intended?");
        }
    }

    public void Heal(GameObject source, float amount)
    {
        if(amount > 0)
        {
            currentHealth += amount;

            //Flag that this game object has been healed
            onHealthDamage?.Invoke(amount, false);

            // Ensure that current maxHealth does not exceed max health
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        } else if(amount < 0)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"Tried to heal {name} but the value was negative ({amount})! Are you sure this is intended?");
        }
        
    }

    //Updates the max health to new value, 
    //Optionally heals the actor to new value based on "heals"
    public void SetMaxHealth(float maxHealth, bool heals  = false)
    {
        this.maxHealth = maxHealth;
        if (heals)
        {
            currentHealth = maxHealth;
        }
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}
