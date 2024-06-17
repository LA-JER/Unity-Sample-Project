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
    public int currentStacks = 1;

    public Buff(int ID, StatManager.Stat targetStat, Item.Operation operation, float modifier, int maxStacks, float duration)
    {
        this.maxStacks = maxStacks;
        this.ID = ID;
        affectedStats.Add(targetStat, new Operand(operation, modifier));
        this.duration = duration;
    }

    public bool AddTargetStat(Stat targetStat, Operation operation, float modifier)
    {
        if (affectedStats.ContainsKey(targetStat)) { return false; }
        affectedStats.Add(targetStat, new Operand(operation, modifier));
        return true;
    }

    public override string ToString()
    {
        return $"Buff ( {ID} )";
    }
}
