using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHitEffect : HitEffect
{
    [SerializeField] private List<GameObject> prefabs = new List<GameObject>();
    [Tooltip("If true, spawns the prefabs on the projectile's position. Otherwise, spawns prefabs on the hit OBJ's position")]
    [SerializeField] private bool spawnOnOwner;
    [Tooltip("Maximum amount of times the prefab can be spawned per projectile instance")]
    public int maxSpawnCount = 1;
    public float damage = 3f;

    private int spawnCount = 0;
    private DamageElement owner;

    public override void DoThing(DamageElement owner, GameObject hitObject)
    {
        this.owner = owner;
        if (spawnCount >= maxSpawnCount) return;

        //Spawns prefabs on Owner or hit object, as set by the field spawnOnOwner
        Transform spawnPosition = spawnOnOwner ? owner.transform : hitObject.transform;
        if(spawnPosition == null ) 
        {
            Debugger.Log(Debugger.AlertType.Error, $"Tried to spawn prefabs but designated spawn point was null! Spawn on owner is {spawnOnOwner}");
            return;
        }
        List<GameObject> instantiatedPrefabs = SpawnPrefabs(spawnPosition.position);



            spawnCount++;
        
        
    }

    private List<GameObject> SpawnPrefabs(Vector2 spawn)
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;
            
            GameObject obj = Instantiate(prefab);
            obj.transform.position = spawn;

            DamageElement damageDealer = obj.GetComponent<DamageElement>();
            if (damageDealer != null)
            {
                damageDealer.source = owner.source;
            }
            list.Add(obj);

            //initializes damage to what this script has defined
            SetDamage(damageDealer);
            
        }

        return list;
    }

    void SetDamage(DamageElement damageDealer)
    {
        if (damageDealer == null) return;

        StatManager statManager = damageDealer.GetComponent<StatManager>();
        if (statManager == null) return;

        statManager.UpdateStat(StatManager.Stat.damage, damage);

    }
}
