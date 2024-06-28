using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHurtCondition : ItemCondition
{
    // Start is called before the first frame update
    void Start()
    {
        HomeBase.OnBaseDamaged += HomeBase_OnBaseDamaged;
    }

    private void HomeBase_OnBaseDamaged()
    {
        Trigger();
    }

    private void OnDestroy()
    {
        HomeBase.OnBaseDamaged -= HomeBase_OnBaseDamaged;
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        return;
    }
}
