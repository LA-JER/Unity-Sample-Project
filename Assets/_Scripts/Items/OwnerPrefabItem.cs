using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//spawns prefabs on the turret owner
public class OwnerPrefabItem : Item
{
    public GameObject prefab;
    public bool randomizedSpawn;
    public float distanceMin;
    public float distanceMax;

    private Transform spawn;

    private void Start()
    {
        if (spawn == null && owner != null)
        {
            spawn = owner.transform;
        }
    }

    public override void Activate()
    {
        if (prefab == null || spawn == null)
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} did not have prefab or spawn set!");
            return;
        }

        GameObject prefabInstance = Instantiate(prefab, spawn);
        if (randomizedSpawn)
        {
            prefabInstance.transform.position = RandomizedPosition();
        }

        DamageElement damageElement = prefabInstance.GetComponent<DamageElement>();
        if(damageElement != null)
        {
            damageElement.Initialize(owner, null, null);
        }
    }

    private Vector2 RandomizedPosition()
    {
        Vector2 startPOS = spawn.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float distance = Random.Range( distanceMin, distanceMax );
        randomDirection *= distance;
        return startPOS + randomDirection;
    }

    public override void TranferOwnership(GameObject newOwner)
    {
        

        
            spawn = newOwner.transform;
        
        owner = newOwner;
        TransferConditions(newOwner);
    }
}
