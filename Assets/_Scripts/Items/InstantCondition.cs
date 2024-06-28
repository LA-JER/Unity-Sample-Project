using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantCondition : ItemCondition
{
    public override void TransferOwnership(GameObject newOwner)
    {
        Trigger();
    }

    // Start is called before the first frame update
    void Start()
    {
        Trigger();
    }
}
