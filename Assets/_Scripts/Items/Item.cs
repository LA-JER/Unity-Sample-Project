using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public abstract class Item : MonoBehaviour, IHoverInfo
{
    //public delegate void EquipmentReadyHandler( Buff buff, bool isReady);
    //public event EquipmentReadyHandler OnEquipmentReady;

    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    public GameObject owner;
    //public Buff buff;

    private ItemCondition[] conditions;
    private int conditionsMet = 0;
    public enum Operation
    {
        Additive,
        Multiplicative,
        Set,
    }

    private void Awake()
    {
        conditions = GetComponents<ItemCondition>();

        if(conditions != null)
        {
            foreach(ItemCondition condition in conditions)
            {
                condition.OnConditionsChange += Condition_OnConditionsChange;
            }
        }
    }

    private void Condition_OnConditionsChange(bool ready)
    {
        if(ready)
        {
            conditionsMet++;
        } else
        {
            conditionsMet--;
        }

        conditionsMet = Mathf.Clamp(conditionsMet, 0, conditions.Length);

        if(conditionsMet == conditions.Length)
        {
            //OnEquipmentReady?.Invoke( buff, true);
            Activate();
            conditionsMet = 0;
        }
    }

    public abstract void Activate();
    public abstract void TranferOwnership(GameObject newOwner);

    private void OnDestroy()
    {
        if (conditions != null)
        {
            foreach (ItemCondition condition in conditions)
            {
                condition.OnConditionsChange -= Condition_OnConditionsChange;
            }
        }
    }

    public string GetName()
    {
       return itemName;
    }

    public string GetDescription()
    {
        return itemDescription;
    }

    public Sprite GetIcon()
    {
        return itemIcon;
    }

    public void TransferConditions(GameObject newOwner)
    {
        foreach(ItemCondition itemCondition in conditions)
        {
            itemCondition.TransferOwnership(newOwner);
        }
        conditionsMet = 0;
    }
}
