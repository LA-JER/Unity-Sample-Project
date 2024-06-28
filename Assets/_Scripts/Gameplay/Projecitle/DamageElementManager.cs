using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DamageElementManager : MonoBehaviour
{
    public delegate void ProjectileShoot(Projectile projectile);
    public event ProjectileShoot OnShootProjectile;

    public delegate void ExplosionShoot(Explosion explosion);
    public event ExplosionShoot OnFireExplosion;

    [SerializeField] private List<GameObject> effectPrefabs = new List<GameObject>();
    [SerializeField] private GameObject projectileMovementPrefab;

    public void AddNewModules(List<GameObject> prefabs)
    {
        if(prefabs != null)
        {
            foreach(GameObject prefab in prefabs)
            {
                if (prefab.GetComponent<HitEffect>() != null)
                {
                    
                    HitEffect effect = prefab.GetComponent<HitEffect>();
                    //check to make sure this is indeed a hit effect
                    if (effect != null)
                    {
                        if (!effectPrefabs.Contains(prefab))
                        {
                            effectPrefabs.Add(prefab);
                        }
                    }
                } else if(prefab.GetComponent<ProjectileMovement>() != null)
                {
                    ProjectileMovement move = prefab.GetComponent<ProjectileMovement>();
                    //check to make sure this is indeed a hit effect
                    if (move != null)
                    {
                        if (projectileMovementPrefab == null)
                        {
                            projectileMovementPrefab = prefab;
                        } else
                        {
                            Debugger.Log(Debugger.AlertType.Warning, "Tried to set a projectile movement but one was already. Are you sure this is intended?");
                        }
                    }
                }
                
            }


            
        }
    }

    public void InitializeModules(DamageElement damage)
    {
        if (damage == null) return;
        
        //Add the hit effects 
        foreach(GameObject prefab in effectPrefabs)
        {
            GameObject effectInstance = Instantiate(prefab, damage.transform);
            damage.AddEffect(effectInstance);
        }

        //if this is a projectile, add its new movement
        if(projectileMovementPrefab != null && damage is Projectile)
        {
            Projectile projectile = (Projectile)damage;

            GameObject instance = Instantiate(projectileMovementPrefab, projectile.transform);
            ProjectileMovement projectileMovement = instance.GetComponent<ProjectileMovement>();
            if(projectileMovement != null)
            {
                projectile.projectileMovement = projectileMovement;
                projectileMovement.projectile = projectile;
            }

            
        } 
        if(damage is Projectile)
        {
            OnShootProjectile?.Invoke(damage as Projectile);
        }
        
        if(damage is Explosion)
        {
            OnFireExplosion?.Invoke(damage as Explosion);
        }

        
    }

    public static void InitializeModules(List<GameObject> hitEffects, GameObject movement, Projectile projectile)
    {
        if (projectile == null) return;

        if(hitEffects != null)
        {
            foreach (GameObject prefab in hitEffects)
            {
                GameObject effectInstance = Instantiate(prefab, projectile.transform);
                projectile.AddEffect(effectInstance);
            }
        }
        

        if (movement != null)
        {
            GameObject instance = Instantiate(movement, projectile.transform);
            ProjectileMovement projectileMovement = instance.GetComponent<ProjectileMovement>();
            if (projectileMovement != null)
            {
                projectile.projectileMovement = projectileMovement;
                projectileMovement.projectile = projectile;
            }


        }

    }

    public GameObject GetProjectileMovement() { return projectileMovementPrefab; }
    public List<GameObject> GetEffectPrefabs()
    {
        List<GameObject> list = new List<GameObject>();
        foreach (GameObject effect in effectPrefabs)
        {
            if(effect == null) continue;
            list.Add(effect);
        }


        return list;
    }


}
