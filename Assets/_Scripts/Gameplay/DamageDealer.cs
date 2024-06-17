using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public abstract class DamageDealer : MonoBehaviour
{
    public delegate void DamageDealt(GameObject source, DamageInstance damage);
    public static event DamageDealt OnDamageDealt;

    public GameObject source;

    public abstract void Initialize(GameObject source, StatManager other, bool setSizeAtStart = false);

    public DamageInstance DealDamage(GameObject source, StatManager statManager, Health other)
    {
        if(other == null) return null;
        if(statManager == null) return null;

        DamageInstance dmg = CalculateDamage(statManager);
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} is dealing [{dmg.damage}, {dmg.isCritical}] to {other.gameObject.name} ");
        other.Damage(source, dmg.damage, dmg.isCritical);
        OnDamageDealt?.Invoke(source, dmg);
        return dmg;
    }
    private DamageInstance CalculateDamage(StatManager statManager)
    {
        if (statManager == null)
        {
            return null;
        }

        float damage = statManager.GetStat(Stat.damage);
        float critChance = statManager.GetStat(Stat.critChance);
        float critRatio = statManager.GetStat(Stat.critRatio);
        bool isCritical = Random.value < critChance;

        if (isCritical)
        {
            damage *= critRatio;
        }

        return new DamageInstance(damage, isCritical);

    }

    public class DamageInstance
    {
        public float damage = 0;
        public bool isCritical = false;

        public DamageInstance()
        {
            damage = 0;
            isCritical = false;
        }

        public DamageInstance(float damage)
        {
            this.damage = damage;
            isCritical = false;
        }
        public DamageInstance(float damage, bool isCritical)
        {
            this.damage = damage;
            this.isCritical = isCritical;
        }

        public void SetIsCritical(bool isCritical)
        {
            this.isCritical = isCritical;
        }
    }
}
