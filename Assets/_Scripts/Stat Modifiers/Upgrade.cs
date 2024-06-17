using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using static StatManager;

public abstract class Upgrade : ScriptableObject, IHoverInfo, IPurchaseAble
{
    public Sprite icon;
    public string upgradeName;
    public string upgradeDescription;
    public int price;
    [SerializedDictionary("Stat", "Operand")]
    public SerializedDictionary<Stat, Operand> statDictionary = new SerializedDictionary<Stat, Operand>();

    public abstract void ApplyUpgrade(GameObject owner);
    public string GetName()
    {
        return upgradeName;
    }

    public string GetDescription()
    {
        return upgradeDescription;
    }

    public Sprite GetIcon()
    {
        return icon;
    }
    public int GetPrice()
    {
        return price;
    }

    public void Activate(GameObject owner = null)
    {

        ApplyUpgrade(owner);

    }
}
