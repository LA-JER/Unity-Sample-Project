using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> effectPrefabs = new List<GameObject>();
    [SerializeField] private GameObject projectileMovementPrefab;

    public void AddNewModules(List<GameObject> prefabs)
    {
        if(prefabs != null)
        {
            foreach(GameObject prefab in prefabs)
            {
                if (prefab.GetComponent<ProjectileHitEffect>() != null)
                {
                    
                    ProjectileHitEffect effect = prefab.GetComponent<ProjectileHitEffect>();
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

    public void InitializeModules(Projectile projectile)
    {
        if(projectile != null)
        {
            foreach(GameObject prefab in effectPrefabs)
            {
                GameObject effectInstance = Instantiate(prefab, projectile.transform);
                projectile.AddEffect(effectInstance);
            }

            if(projectileMovementPrefab != null)
            {
                GameObject instance = Instantiate(projectileMovementPrefab, projectile.transform);
                ProjectileMovement projectileMovement = instance.GetComponent<ProjectileMovement>();
                if(projectileMovement != null)
                {
                    projectile.projectileMovement = projectileMovement;
                    projectileMovement.projectile = projectile;
                }

                
            }
        }


    }
}
