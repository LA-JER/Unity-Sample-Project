using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

//[CreateAssetMenu(menuName = "Tower Game/ UpgradeNode")]
public class Item : MonoBehaviour
{
    public delegate void EquipmentReadyHandler(Item source, IHoverInfo owner, int ID, Stat stat, Operation operation, float modifier, int maxStacks, float duration = -1);
    public static event EquipmentReadyHandler OnEquipmentReady;
    public delegate void EquipmentUnready(Item source, IHoverInfo owner, int ID, Stat stat);
    public static event EquipmentUnready OnEquipmentRevoke;

    public enum Operation
    {
        Additive,
        Multiplicative,
        Set,
    }

    public Operation operation;
    public float modifier;
    public List<MonoBehaviour> behaviours;
    //public int ID = 0;
    private ICondition[] allConditions;
    private List<ICondition> positiveConditions = new List<ICondition>();
    private int conditionsReady = 0;



}
