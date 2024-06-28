using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public class StatItem : Item
{
    [SerializedDictionary("Stat", "Modifier")]
    [SerializeField] public SerializedDictionary<Stat, Operand> statChanges = new SerializedDictionary<Stat, Operand>();

    public override void Activate()
    {
        if (owner == null)
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} did not have an owner set!");
            return;
        }

        BuffManager buffManager = owner.GetComponent<BuffManager>();
        if (buffManager == null) return;

        foreach ( var statChange in statChanges)
        {
            buffManager.ApplyPermanentBuff(statChange.Key, statChange.Value.operation, statChange.Value.modifier);
        }
        
    }

    public override void TranferOwnership(GameObject newOwner)
    {
        owner = newOwner;
        TransferConditions(newOwner);
        
    }
}
