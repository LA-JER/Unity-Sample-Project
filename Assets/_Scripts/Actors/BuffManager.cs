using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;
using static Item;
using System;
using AYellowpaper.SerializedCollections;

public class BuffManager : MonoBehaviour
{
    public delegate void BuffApply(int buffID, bool isActive);
    public event BuffApply OnBuffChange;

    //private Dictionary<int, Buff> buffs = new Dictionary<int, Buff>();
    [SerializedDictionary("ID", "Buff")]
    public SerializedDictionary<int, Buff> buffs = new SerializedDictionary<int, Buff>();
    [SerializedDictionary("ID", "Time Left")]
    public SerializedDictionary<int, float> buffTimeLeft = new SerializedDictionary<int, float>();
    [SerializedDictionary("Stat", "Base Value")]
    public SerializedDictionary<Stat, float> baseStats = new SerializedDictionary<Stat, float>();

    private StatManager statManager;
    private Dictionary<int, Coroutine> buffTimers = new Dictionary<int, Coroutine>();   

    // Start is called before the first frame update
    void Awake()
    {
        statManager = GetComponent<StatManager>();

        InitializeBaseStats();
    }


    public void ApplyPermanentBuff(Stat targetStat, Item.Operation operation, float value)
    {
        
        if (!statManager.HasStat(targetStat))
        {
            statManager.AddStat(targetStat, value);
        } 
        float oldVal = statManager.GetStat(targetStat);

        switch (operation)
            {
                case Item.Operation.Multiplicative:
                    statManager.UpdateStat(targetStat, oldVal * value);
                    break;
                case Item.Operation.Additive:
                    statManager.UpdateStat(targetStat, oldVal + value);
                    break;
                case Item.Operation.Set:
                    statManager.UpdateStat(targetStat, value);
                    break;
            }
        UpdateBaseStats(targetStat);

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

    public void ApplyTimedBuff(Buff buff)
    {
        if (buff.duration <= 0 || buff.ID == 0)
        {
            return;
        }

        
        if (buffs.ContainsKey(buff.ID))
        {
            

            //re apply buff
            if (buffs[buff.ID].currentStacks + 1 <= buff.maxStacks)
            {
                buffs[buff.ID].currentStacks++;
                //stop old timer
                Coroutine oldTimer = buffTimers[buff.ID];
                if (oldTimer != null)
                {
                    StopCoroutine(oldTimer);
                }
                buffTimers.Remove(buff.ID);

                //start new timer
                Buff internalBuff = buffs[buff.ID];
                Coroutine timer = StartCoroutine(BuffTimer(internalBuff));
                buffTimers.Add(buff.ID, timer);
            }

            else if (buffs[buff.ID].currentStacks == buff.maxStacks && buff.isRefreshable)
            {
                //stop old timer
                Coroutine oldTimer = buffTimers[buff.ID];
                if (oldTimer != null)
                {
                    StopCoroutine(oldTimer);
                }
                buffTimers.Remove(buff.ID);

                //start new timer
                Buff internalBuff = buffs[buff.ID];
                Coroutine timer = StartCoroutine(BuffTimer(internalBuff));
                buffTimers.Add(buff.ID, timer);
            }

        }
        //add the buff for the first time
        else
        {
            
            Buff buffInstance = new Buff(buff);
            buffs.Add(buff.ID, buffInstance);
            buffs[buff.ID].currentStacks++;
            buffTimeLeft.Add(buff.ID, buff.duration);
            OnBuffChange?.Invoke(buff.ID, true);

            Coroutine timer = StartCoroutine(BuffTimer(buff));
            buffTimers.Add(buff.ID, timer);
        }

        
    }

    //A timer which updates stats when the buff is first applied and also when removed
    private IEnumerator BuffTimer(Buff newBuff)
    {
        

        //since buffs can modify multiple stats, ensure we recalculate each stat
        foreach (Stat targetStat in newBuff.affectedStats.Keys)
        {
            
            ReCalculateStat(targetStat);
        }
            
        //wait the buff duration

        for(int i = (int)newBuff.duration; i > 0; i--)
        {
            buffTimeLeft[newBuff.ID] = i;
            yield return new WaitForSeconds(1);
            
        }
            
      


        //decrement the buff stacks. If it reaches 0, remove buff entirely
        if (buffs.ContainsKey(newBuff.ID))
        {
            
                buffs[newBuff.ID].currentStacks--;
            
            if(buffs[newBuff.ID].currentStacks >0 || !newBuff.decays)
            {
               
                buffTimers.Remove(newBuff.ID);
                buffTimeLeft.Remove(newBuff.ID);
                buffs.Remove(newBuff.ID);
                OnBuffChange?.Invoke(newBuff.ID, false);
            }


        }

        //recalculate stats on buff exit as well
        foreach (Stat targetStat in newBuff.affectedStats.Keys)
        {
            ReCalculateStat(targetStat);
        }


        //starts a new timer for the buff with 1 fewer stack
        if(newBuff.decays &&  newBuff.currentStacks > 1)
        {
            Coroutine decay = StartCoroutine(BuffTimer(newBuff));
            buffTimers[newBuff.ID] = decay;
        }

        

        yield break;

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
        List<Buff> setBuffs = new List<Buff>();

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
                    else if (buff.affectedStats[stat].operation == Item.Operation.Set)
                    {
                        setBuffs.Add(buff);
                        Debugger.Log(Debugger.AlertType.Error, "Set buff with duration not supported!");
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
            if (buff.affectedStats[stat].modifier == 0)
            {
                newVal = 0;
                break;
            }
            baseVal *= buff.affectedStats[stat].modifier;
            newVal = baseVal;
        }
        

        statManager.UpdateStat(stat, newVal);
    }

    private void OnDestroy()
    {
        OnBuffChange = null;
    }

  
}
