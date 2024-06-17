using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;
using static UnityEngine.EventSystems.EventTrigger;

public class BuffHitEffect : ProjectileHitEffect
{
    [Tooltip("A list of buffs to apply to the projectile")]
    [SerializeField] private List<Buff> ownerBuffs = new List<Buff>();
    [Tooltip("A list of buffs to apply to the hit object")]
    [SerializeField] private List<Buff> hitBuffs = new List<Buff>();


    public override void DoThing(Projectile projectile, GameObject hitObject)
    {
       // Debugger.Log(Debugger.AlertType.Info, $"Calling buff hit effect do thing!");
        if (projectile != null)
        {
            BuffManager buffManager = projectile.GetComponent<BuffManager>();
            if(buffManager != null)
            {
                foreach(Buff buff in ownerBuffs)
                {
                    Debugger.Log(Debugger.AlertType.Verbose, $"Applying {buff} to {projectile}");
                    buffManager.ApplyTimedBuff(buff);
                }
            }
        }

        if (hitObject != null)
        {
            BuffManager buffManager = hitObject.GetComponent<BuffManager>();
            if (buffManager != null)
            {
                foreach (Buff buff in hitBuffs)
                {
                    Debugger.Log(Debugger.AlertType.Verbose, $"Applying {buff} to {hitObject}");
                    buffManager.ApplyTimedBuff(buff);
                }
            }
        }
    }

}
