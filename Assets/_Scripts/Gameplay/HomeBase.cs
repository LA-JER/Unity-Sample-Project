using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : MonoBehaviour
{
    public delegate void BaseHealthChange();
    public static event BaseHealthChange OnBaseDie;
    public static event BaseHealthChange OnBaseDamaged;

    private Health Health;
    // Start is called before the first frame update
    void Start()
    {
        Health = GetComponent<Health>();
        Health.onHealthZero += Health_onHealthZero;
        Health.onHealthDamage += Health_onHealthDamage;
    }

    private void Health_onHealthDamage(float amount, bool isCritical)
    {
        OnBaseDamaged?.Invoke();
    }

    private void Health_onHealthZero(GameObject source)
    {
        Debugger.Log(Debugger.AlertType.Info, "Home is Destroyed!");
        OnBaseDie?.Invoke();
        //Health.enabled = false;
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
