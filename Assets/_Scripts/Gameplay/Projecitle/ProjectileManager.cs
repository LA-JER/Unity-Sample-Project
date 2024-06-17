using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> effectPrefabs = new List<GameObject>();

    public void AddNewEffect(GameObject effectPrefab)
    {
        if(effectPrefab != null)
        {
            ProjectileHitEffect effect = effectPrefab.GetComponent<ProjectileHitEffect>(); 
            //check to make sure this is indeed a hit effect
            if(effect != null)
            {
                if (!effectPrefabs.Contains(effectPrefab))
                {
                    effectPrefabs.Add(effectPrefab);
                }
            }
        }
    }

    public void ApplyEffects(Projectile projectile)
    {
        if(projectile != null)
        {
            foreach(GameObject prefab in effectPrefabs)
            {
                GameObject effectInstance = Instantiate(prefab, projectile.transform);
                projectile.AddEffect(effectInstance);
            }
        }
    }
}
