using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Base class for upgrade conditions, which can trigger an item effect based on a variety of self defined criteria
public abstract class ItemCondition : MonoBehaviour
{
    public delegate void ConditionChangeHandler( bool ready);
    public event ConditionChangeHandler OnConditionsChange;
    //public event ConditionChangeHandler onConditionsFail;
    [Tooltip("Determines whether this removes buffs from actor when this condition is met")]
    //public bool isNegativeCondition = false;

  

    public void Trigger( bool invoke = true)
    {
        //if (isNegativeCondition)
       // {
       // //    OnConditionsChange?.Invoke( false);
       // }
       // else
       // {
            OnConditionsChange?.Invoke( invoke);
       // }
    }

    public abstract void TransferOwnership(GameObject newOwner);
}
