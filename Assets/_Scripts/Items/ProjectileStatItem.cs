using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public class ProjectileStatItem : Item
{
    [SerializedDictionary("Projectile Stat", "Modifier")]
    [SerializeField] public SerializedDictionary<Stat, Operand> statChanges = new SerializedDictionary<Stat, Operand>();


    private DamageElementManager projectileManager;
    private Projectile projectileToBeAffected;
    public bool affectNext = false;
    private void Start()
    {
        projectileManager = owner.GetComponent<DamageElementManager>();
        if (projectileManager != null)
        {
            projectileManager.OnShootProjectile += ProjectileManager_OnShoot;
        }
    }

    private void ProjectileManager_OnShoot(Projectile projectile)
    {
        projectileToBeAffected = projectile;
        StartCoroutine(Safety());
        if(affectNext && projectileToBeAffected != null)
        {
            AffectProjectile(projectileToBeAffected);
            projectileToBeAffected = null;
            affectNext = false;
        }
    }

    public override void Activate()
    {
        //Debug.Log($"calling activate. {projectileToBeAffected} , affect next is {affectNext}");

        if(projectileToBeAffected != null)
        {
            AffectProjectile(projectileToBeAffected);
        } else
        {
            affectNext = true;
        }


    }

    private void AffectProjectile(Projectile projectile)
    {
        BuffManager buffManager = projectile.GetComponent<BuffManager>();
        if (buffManager != null)
        {
            foreach (var statChange in statChanges)
            {
                buffManager.ApplyPermanentBuff(statChange.Key, statChange.Value.operation, statChange.Value.modifier);
            }
        }
    }

    public override void TranferOwnership(GameObject newOwner)
    {
        
        projectileManager.OnShootProjectile -= ProjectileManager_OnShoot;


        owner = newOwner;
        projectileManager = newOwner.GetComponent<DamageElementManager>();
        if (projectileManager != null)
        {
            projectileManager.OnShootProjectile += ProjectileManager_OnShoot;
        }
        affectNext = false;
        TransferConditions(newOwner);
    }
    //prevents this script from accessing a projectile too deep into its lifespan
    private IEnumerator Safety()
    {
        yield return null;
        yield return null;
        yield return null;
        projectileToBeAffected = null;
    }

    private void OnDestroy()
    {
        if (projectileManager != null)
        {
            projectileManager.OnShootProjectile += ProjectileManager_OnShoot;
        }
    }
}
