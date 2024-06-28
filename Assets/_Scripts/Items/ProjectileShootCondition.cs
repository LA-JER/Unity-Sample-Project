using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShootCondition : ItemCondition
{
    public int shotsNeeded = 3;


    private int shots = 0;
    private Item parentItem;
    private GameObject owner;
    private DamageElementManager manager;

    private void Start()
    {
        parentItem = GetComponent<Item>();
        if (parentItem == null) return;
        owner = parentItem.owner;

        manager = owner.GetComponent<DamageElementManager>();
        if(manager != null)
        {
            manager.OnShootProjectile += Manager_OnShoot;
        }
    }

    private void Manager_OnShoot(Projectile projectile)
    {
        shots++;
        if(shots > shotsNeeded)
        {
            Trigger();
            shots = 0;
        }
    }

    private void OnDestroy()
    {

        if (manager != null)
        {
            manager.OnShootProjectile -= Manager_OnShoot;
        }
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        if (newOwner == owner) 
        {
            Debugger.Log(Debugger.AlertType.Warning, "new owner was null!");
            return;
        }

        manager.OnShootProjectile -= Manager_OnShoot;
        owner = newOwner;

        manager = owner.GetComponent<DamageElementManager>();
        manager.OnShootProjectile += Manager_OnShoot;

        shots = 0;
    }
}
