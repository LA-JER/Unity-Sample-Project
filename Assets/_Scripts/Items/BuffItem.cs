using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffItem : Item
{
    public Buff buff;

    public override void Activate()
    {
        if (owner == null)
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} did not have an owner set!");
            return;
        }

        BuffManager buffManager = owner.GetComponent<BuffManager>();
        if (buffManager == null) return;
        buffManager.ApplyTimedBuff(buff);
    }

    public override void TranferOwnership(GameObject newOwner)
    {
        owner = newOwner;
        TransferConditions(newOwner);
        

    }
}
