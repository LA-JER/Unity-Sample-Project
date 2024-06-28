using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Tooltip("Adds a chance factor to other conditions. Requires at least one other non random item condition to work")]
public class RandomCondition : ItemCondition
{
    [Tooltip("Roll must be less than or equal to this. Max value of 1")]
    public float chance = .5f;

    private ItemCondition[] conditions;


    private void Awake()
    {
        conditions = GetComponents<ItemCondition>();

        if (conditions != null)
        {
            foreach (ItemCondition condition in conditions)
            {
                if(condition == this) { continue; }

                condition.OnConditionsChange += Condition_OnConditionsChange;
            }
        }
    }

    private void Condition_OnConditionsChange(bool ready)
    {
        float roll = Random.Range(0f, 1f);
        if(roll <= chance)
        {
            Trigger();
        } 
        //if this fails the roll, reset all conditions
        else
        {
            for(int i  = 0; i < conditions.Length; i++)
            {
                Trigger(false);
            }
        }
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        return;
    }

    private void OnDestroy()
    {
        if (conditions != null)
        {
            foreach (ItemCondition condition in conditions)
            {
                if (condition == this) { continue; }
                condition.OnConditionsChange -= Condition_OnConditionsChange;
            }
        }
    }
}
