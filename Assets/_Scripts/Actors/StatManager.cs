using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatManager : MonoBehaviour
{
    public enum Stat
    {
        maxHealth,
        damage,
        moveSpeed,
        lifeSpan, //life span of child instantiated by actor
        rotateSpeed,// Speed at which the pivot rotates
        fireRate,// Rate of fire in shots per second
        pierceCount,
        sizeX,
        sizeY,
        fireCount, //amount of times the actor fires consecutively
        critChance,
        critRatio,
        range,
        coneAngle, //projectile spread
        projectileSpeed,
        shopPriceModifier,
        shopItemCount,
        level,
        critBounce,
        targetMax,
        NONE,
        firePoints,
        trackingStrength,
        trackingSize,

    }

    [SerializedDictionary("Stat", "Value")]
    public SerializedDictionary<Stat, float> statDictionary = new SerializedDictionary<Stat, float>();

    private Health Health;
    /*
    public void AddStat(Stat stat)
    {
        if (!statDictionary.ContainsKey(stat))
        {
           // statDictionary.Add(stat, statDefaultValues[stat]);

        }
    }*/

    private void Start()
    {
        Health = GetComponent<Health>();
        if(Health != null )
        {
            Health.SetMaxHealth(GetStat(Stat.maxHealth));
        }
    }


    public void AddStat(Stat stat, float value)
    {
        if (!statDictionary.ContainsKey(stat))
        {
            statDictionary.Add(stat, value);
        } else
        {
            Debugger.Log(Debugger.AlertType.Error, $"Tried to add stat '{stat}' to '{name}' but it already has it!");
        }
    }


    /// <summary>
    /// Given a StatManager other, overwrites this object's stat values with those of other's wherever they both share the same stat
    /// </summary>
    /// <param name="other"></param>
    public void UpdateApplicableStatsFrom(StatManager other)
    {
        if (other != null)
        {
            foreach (Stat otherStat in other.statDictionary.Keys)
            {
                if (statDictionary.ContainsKey(otherStat))
                {
                    statDictionary[otherStat] = other.statDictionary[otherStat];
                }
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} was given a null StatManager to copy stats from");
        }
    }
    public void UpdateStat(Stat stat, float newValue)
    {
        if (statDictionary.ContainsKey(stat))
        {
            statDictionary[stat] = newValue;
        }
        else
        {

            //AddStat(stat, newValue);
            Debugger.Log(Debugger.AlertType.Error, $"Tried to modify stat '{stat}' on '{name}' but it does not exist!");
        }
    }

    public float GetStat(Stat stat)
    {
        if (statDictionary.Keys.Contains(stat))
        {
            return Mathf.Clamp(statDictionary[stat], 0 , Mathf.Infinity);
        }
        else
        {
            Debugger.Log(Debugger.AlertType.Error, $"Tried getting value of stat '{stat}' from '{name}' but it doesn't have it!");
            return -1;
        }

    }

    public bool HasStat(Stat stat)
    {
        if (statDictionary.Keys.Contains(stat))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public override string ToString()
    {
        string st = $"{name}'s Stat Manager with the following stats: [ \n";
        foreach (Stat stat in statDictionary.Keys)
        {
            st += " " + stat.ToString() + " : " + statDictionary[stat] + ",";
        }
        st += " ]";


        return st;
    }
}
