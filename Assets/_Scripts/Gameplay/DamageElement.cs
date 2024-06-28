using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ITargetable;
using static StatManager;

public abstract class DamageElement : MonoBehaviour
{
    public delegate void DamageDealt(GameObject source, GameObject hit, DamageInstance damage);
    public static event DamageDealt OnDamageDealt;

    public GameObject source;
    public SerializedDictionary<PlayArea, bool> targetsCapable = new SerializedDictionary<PlayArea, bool>();
    public List<GameObject> effects = new List<GameObject>();
    public abstract void Initialize(GameObject source, StatManager other, Dictionary<PlayArea, bool> targetsCapable, bool setSizeAtStart = false);

    public DamageInstance DealDamage(GameObject source, StatManager statManager, GameObject hit)
    {
        if(hit == null) return null;
        if(statManager == null) return null;

        DamageInstance dmg = CalculateDamage(statManager);
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} is dealing [{dmg.damage}, {dmg.isCritical}] to {hit.gameObject.name} ");

        Health other  = hit.GetComponent<Health>();
        if(other == null) return null;

        other.Damage(source, dmg.damage, dmg.isCritical);
        OnDamageDealt?.Invoke(source, hit, dmg);
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
    public bool CanTarget(PlayArea otherArea)
    {
        if (targetsCapable == null || !targetsCapable.ContainsKey(otherArea))
        {
            Debugger.Log(Debugger.AlertType.Error, $"Did not have {otherArea} in dictionary! Did you forget to add it to the turret?");
            return false;
        }

        return targetsCapable[otherArea];
    }

    public void AddEffect(GameObject effect)
    {
        if (effect != null)
        {
            if (!effects.Contains(effect))
            {
                effects.Add(effect);
            }
        }
    }


    public void DoEffects(GameObject hit)
    {
        foreach (GameObject effect in effects)
        {
            HitEffect hitEffect = effect.GetComponent<HitEffect>();
            if (hitEffect != null)
            {
                hitEffect.DoThing(this, hit);
            }

        }
    }

}
