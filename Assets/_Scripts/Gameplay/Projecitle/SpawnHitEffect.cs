using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHitEffect : ProjectileHitEffect
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [Tooltip("If true, spawns the prefabs on the projectile's position. Otherwise, spawns prefabs on the hit OBJ's position")]
    [SerializeField] private bool spawnOnOwner;
    [Tooltip("Maximum amount of times the prefab can be spawned per projectile instance")]
    public int maxSpawnCount = 1;

    private int spawnCount = 0;

    public override void DoThing(Projectile owner, GameObject hitObject)
    {
        if(spawnCount <  maxSpawnCount)
        {
            if (spawnOnOwner)
            {
                if (owner != null)
                {
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab != null)
                        {
                            GameObject obj = Instantiate(prefab);
                            obj.transform.position = owner.transform.position;

                            DamageDealer damageDealer = obj.GetComponent<DamageDealer>();
                            if(damageDealer != null)
                            {
                                damageDealer.source = owner.source;
                            }
                        }
                    }
                }
                else
                {
                    Debugger.Log(Debugger.AlertType.Warning, "Tried to spawn prefabs on owner but the given owner was null");
                }
            }

            else
            {
                if (hitObject != null)
                {
                    foreach (GameObject prefab in prefabs)
                    {
                        if (prefab != null)
                        {
                            GameObject obj = Instantiate(prefab);
                            obj.transform.position = hitObject.transform.position;

                            DamageDealer damageDealer = obj.GetComponent<DamageDealer>();
                            if (damageDealer != null)
                            {
                                damageDealer.source = owner.source;
                            }
                        }
                    }
                }
                else
                {
                    Debugger.Log(Debugger.AlertType.Warning, "Tried to spawn prefabs on hitObject but the given hitObject was null");
                }
            }

            spawnCount++;
        }
        
    }
}
