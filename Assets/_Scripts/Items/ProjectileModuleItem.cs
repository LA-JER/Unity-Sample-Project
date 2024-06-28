using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModuleItem : Item
{
    public List<GameObject> effectPrefabs = new List<GameObject>();
    public GameObject movement;

    private DamageElementManager projectileManager;
    private Projectile projectileToBeAffected;
    private bool affectNext = false;
    private void Start()
    {
        projectileManager = owner.GetComponent<DamageElementManager>();
        if(projectileManager != null)
        {
            projectileManager.OnShootProjectile += ProjectileManager_OnShoot;
        }
    }

    private void ProjectileManager_OnShoot(Projectile projectile)
    {
        projectileToBeAffected = projectile;
        StartCoroutine(Safety());
        if (affectNext && projectileToBeAffected != null)
        {
            DamageElementManager.InitializeModules(effectPrefabs, movement, projectileToBeAffected);
            projectileToBeAffected = null;
            affectNext = false;
        }
    }

    public override void Activate()
    {
        if (projectileToBeAffected != null)
        {
            DamageElementManager.InitializeModules(effectPrefabs, movement, projectileToBeAffected);
        }
        else
        {
            affectNext = true;
        }


    }

    public override void TranferOwnership(GameObject newOwner)
    {
        
        projectileManager.OnShootProjectile -= ProjectileManager_OnShoot;


        owner = newOwner;
        projectileManager = newOwner.GetComponent<DamageElementManager>();
        if(projectileManager != null)
        {
            projectileManager.OnShootProjectile += ProjectileManager_OnShoot;
        }
        affectNext= false;
        TransferConditions(newOwner);
    }

    //prevents this script from accessing a projectile too deep into its lifespan
    private IEnumerator Safety()
    {
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
