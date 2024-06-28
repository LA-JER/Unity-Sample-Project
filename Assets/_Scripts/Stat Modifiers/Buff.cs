using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Item;
using static StatManager;

[Serializable]
public class Buff
{
    public int ID;
    // public StatManager.Stat stat;
    //public Equipment.Operation operation;

    [SerializedDictionary("Stat", "Value")]
    public SerializedDictionary<Stat, Operand> affectedStats = new SerializedDictionary<Stat, Operand>();

    public float duration;
    public int maxStacks;
    public int currentStacks = 0;
    public bool isRefreshable = false;
    public bool decays = false;

    public Buff(int ID, StatManager.Stat targetStat, Item.Operation operation, float modifier, int maxStacks, float duration)
    {
        this.maxStacks = maxStacks;
        this.ID = ID;
        AddTargetStat(targetStat, new Operand(operation, modifier));
        this.duration = duration;
        this.isRefreshable = false;
        currentStacks = 0;
    }
    public Buff(int ID, StatManager.Stat targetStat, Item.Operation operation, float modifier, int maxStacks, float duration, bool refreshable)
    {
        this.maxStacks = maxStacks;
        this.ID = ID;
        AddTargetStat(targetStat, new Operand(operation, modifier));
        this.duration = duration;
        this.isRefreshable = refreshable;
        currentStacks = 0;
    }
    public Buff(int ID, StatManager.Stat targetStat, Item.Operation operation, float modifier, int maxStacks, float duration, bool refreshable, bool decays)
    {
        this.maxStacks = maxStacks;
        this.ID = ID;
        AddTargetStat(targetStat, new Operand(operation, modifier));
        this.duration = duration;
        this.isRefreshable = refreshable;
        currentStacks = 0;
        this.decays = decays;
    }

    public Buff(Buff other)
    {
        maxStacks = other.maxStacks;
        ID = other.ID;

        foreach(var statModification in other.affectedStats)
        {
            AddTargetStat(statModification.Key, statModification.Value);
        }

        
        duration = other.duration;
        isRefreshable = other.isRefreshable;
        currentStacks = 0;
        decays = other.decays;
    }

    public bool AddTargetStat(Stat targetStat, Operation operation, float modifier)
    {
        if (affectedStats.ContainsKey(targetStat)) { return false; }
        affectedStats.Add(targetStat, new Operand(operation, modifier));
        return true;
    }

    public bool AddTargetStat(Stat targetStat, Operand operand)
    {
        if (affectedStats.ContainsKey(targetStat)) { return false; }
        affectedStats.Add(targetStat, operand);
        return true;
    }

    public override string ToString()
    {
        return $"Buff ( {ID} )";
    }
}
