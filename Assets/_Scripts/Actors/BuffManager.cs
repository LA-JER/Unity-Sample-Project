using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;
using static Item;
using System;
using AYellowpaper.SerializedCollections;

public class BuffManager : MonoBehaviour
{
    
    //private Dictionary<int, Buff> buffs = new Dictionary<int, Buff>();
    [SerializedDictionary("ID", "Buff")]
    public SerializedDictionary<int, Buff> buffs = new SerializedDictionary<int, Buff>();
    [SerializedDictionary("Stat", "Base Value")]
    public SerializedDictionary<Stat, float> baseStats = new SerializedDictionary<Stat, float>();

    private StatManager statManager;

    // Start is called before the first frame update
    void Awake()
    {
        

        //Item.OnEquipmentReady += Equipment_OnEquipmentReady;
       // OnEquipmentRevoke += BuffManager_OnEquipmentRevoke;
       // Upgrade.OnUpgradeReady += Upgrade_OnUpgradeReady;
        //XPManager.onLevelUp += XPManager_onLevelUp;
    }

    private void Start()
    {
        statManager = GetComponent<StatManager>();
        InitializeBaseStats();
    }


    /*
    private void Equipment_OnEquipmentReady(Item source, IHoverInfo owner, int ID, Stat targetStat, Item.Operation operation, float modifier, int maxStacks, float duration = -1)
    {
        if (statManager != null && actor != null)
        {
            // Debug.Log($"checking if {actor} is = {owner}");
            if (actor == owner && baseStats.ContainsKey(targetStat))
            {
                //determine if this is a temporary buff (effectDuration > 0) or permanent (effectDuration <= 0)
                if (duration > 0)
                {
                    ApplyTimedBuff(ID, targetStat, operation, modifier, maxStacks, duration);
                }
                else
                {
                    ApplyPermanentBuff(targetStat, operation, modifier);
                }

                //Debug.Log($"Applied {operation} {modifier} on {stat} on {gameObject}");
            }


        }
    }*/



    public void ApplyPermanentBuff(Stat targetStat, Item.Operation operation, float modifier)
    {
        float oldVal = statManager.GetStat(targetStat);

        if (operation == Item.Operation.Multiplicative)
        {
            statManager.UpdateStat(targetStat, oldVal * modifier);

            UpdateBaseStats(targetStat);

        }
        else if (operation == Item.Operation.Additive)
        {
            statManager.UpdateStat(targetStat, oldVal + modifier);
            UpdateBaseStats(targetStat);
        }

        //special case to for health
        if (targetStat == Stat.maxHealth)
        {
            Health health = GetComponent<Health>();
            if (health)
            {
                float newVal = statManager.GetStat(targetStat);
                health.SetMaxHealth(newVal);
                Debugger.Log(Debugger.AlertType.Warning, "TODO, increasing max health heal!");
                health.Heal(gameObject, newVal - oldVal);

            }
        }

    }


    public void ApplyTimedBuff(Buff buff)
    {
        StartCoroutine(BuffTimer(buff));
    }



    void RemoveBuff(int ID, Stat targetStat)
    {
        if (buffs.ContainsKey(ID))
        {
            Buff buff = buffs[ID];
            buffs.Remove(ID);
            Debug.Log($"removed buff {buff}, {buffs.Count} buffs remain");
            ReCalculateStat();
            // Debug.Log($"Removed {operation} {modifier} on {stat}");
        }
    }

    private void InitializeBaseStats()
    {
        if (statManager != null)
        {
            foreach (Stat stat in statManager.statDictionary.Keys)
            {
                baseStats.Add(stat, statManager.GetStat(stat));
            }
        }
    }
    private void UpdateBaseStats(Stat targetStat)
    {
        if (baseStats.ContainsKey(targetStat))
        {
            baseStats[targetStat] = statManager.GetStat(targetStat);
        }
        else
        {
            baseStats.Add(targetStat, statManager.GetStat(targetStat));
        }
    }

    //A timer which modifies stats when the buff is first applied and also when removed
    private IEnumerator BuffTimer(Buff newBuff)
    {
        if (newBuff.duration > 0 && newBuff.ID != 0)
        {

            //increment the buff stack amounts
            if (!buffs.ContainsKey(newBuff.ID))
            {
                buffs.Add(newBuff.ID, newBuff);
            }
            if (buffs[newBuff.ID].currentStacks < newBuff.maxStacks)
            {
                buffs[newBuff.ID].currentStacks++;
            }

            //since buffs can modify multiple stats, ensure we recalculate each stat
            foreach (Stat targetStat in newBuff.affectedStats.Keys)
            {
                //Debug.Log($"buffing {targetStat}. Was {statManager.GetStat(targetStat)}");
                ReCalculateStat(targetStat);
                //Debug.Log($"Now, {statManager.GetStat(targetStat)}");
            }
            
            //wait the buff duration
            yield return new WaitForSeconds(newBuff.duration);

            //decrement the buff stacks. If it reaches 0, remove buff entirely
            if (buffs.ContainsKey(newBuff.ID))
            {
                if (buffs[newBuff.ID].currentStacks > 1)
                {
                    buffs[newBuff.ID].currentStacks--;
                }
                else
                {
                    buffs.Remove(newBuff.ID);
                }

            }

            //recalculate stats on buff exit as well
            foreach (Stat targetStat in newBuff.affectedStats.Keys)
            {
                ReCalculateStat(targetStat);
            }


        }
    }

    //Applies additive buffs before multiplicative buffs for a specific stat
    //if the target stat is not given, recalculate all stats
    private void ReCalculateStat(Stat targetStat = Stat.NONE )
    {
        if (targetStat == Stat.NONE)
        {
            foreach (Stat stat in baseStats.Keys)
            {
                StatRecalculateHelper(stat);
            }
        }
        else
        {
            StatRecalculateHelper(targetStat);
        }
    }

    void StatRecalculateHelper(Stat stat)
    {
        //distinguish between buffs that are added vs multiplied
        List<Buff> additiveBuffs = new List<Buff>();
        List<Buff> mulitplyBuffs = new List<Buff>();

        foreach (Buff buff in buffs.Values)
        {
            if (buff.affectedStats.ContainsKey(stat))
            {
                for (int i = 0; i < buff.currentStacks; i++)
                {
                    if (buff.affectedStats[stat].operation == Item.Operation.Multiplicative)
                    {
                        mulitplyBuffs.Add(buff);
                    }
                    else if (buff.affectedStats[stat].operation == Item.Operation.Additive)
                    {
                        additiveBuffs.Add(buff);
                    }
                }
            }
        }

        float baseVal = baseStats[stat];
        float newVal = baseStats[stat];

        //apply additive buffs
        foreach (Buff buff in additiveBuffs)
        {
            baseVal += buff.affectedStats[stat].modifier;


            newVal = baseVal;
        }
        //apply multiplicative buffs
        foreach (Buff buff in mulitplyBuffs)
        {
            baseVal *= buff.affectedStats[stat].modifier;
            newVal = baseVal;
        }

        statManager.UpdateStat(stat, newVal);
    }

    private void OnDestroy()
    {
        //Equipment.OnEquipmentReady -= Equipment_OnEquipmentReady;
        //XPManager.onLevelUp -= XPManager_onLevelUp;
       // Upgrade.OnUpgradeReady -= Upgrade_OnUpgradeReady;
        //OnEquipmentRevoke -= BuffManager_OnEquipmentRevoke;
    }

  
}
