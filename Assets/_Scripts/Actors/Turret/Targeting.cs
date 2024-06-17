using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Provides utilities for selecting target enemies based on various styles (closest, strongest, etc.) and range checks.
/// </summary>
public static class Targeting
{
    public enum TargetingStyle
    {
        closest,
        strongest,
        weakest,
        random,
        first,
    }

    private static int randomCheckLimit = 100;

    public static List<Transform> TargetingChip(List<Transform> list, TargetingStyle targetingStyle, int targetingMax, Transform shooter, float range)
    {
        //copy of all targets in range of thsi object
        List<Transform> availableTargets = new List<Transform>();
        //list to keep track of targets actively aiming at
        List<Transform> assignedTargets = new List<Transform>();

        foreach (Transform t in list)
        {
            availableTargets.Add(t);
        }

        for (int i = 0; i < targetingMax; i++)
        {
            Transform target = null;

            //pick target based on targeting style
            switch (targetingStyle)
            {
                case TargetingStyle.closest:
                    target = FindNearestEnemy(availableTargets, shooter, range);
                    break;
                case TargetingStyle.strongest:
                    target = FindStrongestEnemy(availableTargets, shooter, range);
                    break;
                case TargetingStyle.weakest:
                    target = FindWeakestEnemy(availableTargets, shooter, range);
                    break;
                case TargetingStyle.random:
                    target = FindRandomEnemy(availableTargets, shooter, range);
                    break;
                case TargetingStyle.first:
                    target = FindFirstEnemy(availableTargets, shooter, range);
                    break;
                default:
                    break;
            }


            if (target != null)
            {
                assignedTargets.Add(target);
                availableTargets.Remove(target);
            }


        }

        return assignedTargets;
    }

    static Transform FindFirstEnemy(List<Transform> availableTargets, Transform shooter, float range)
    {
        if (availableTargets != null && availableTargets.Count > 0)
        {
            foreach (Transform target in availableTargets)
            {
                if (target != null)
                {
                    float distanceToEnemy = Vector3.Distance(shooter.position, target.transform.position);
                    if ( distanceToEnemy <= range)
                    {
                        return target;
                    }
                }
            }
        }
        return null;
    }

    static Transform FindStrongestEnemy(List<Transform> availableTargets, Transform shooter, float range)
    {
        if (availableTargets != null && availableTargets.Count > 0)
        {
            float highestHealth = 0;
            Transform strongestTarget = null;
            float distanceToEnemy = 0;
            foreach (Transform target in availableTargets)
            {
                if (target != null)
                {
                    distanceToEnemy = Vector3.Distance(shooter.position, target.transform.position);

                    Health health = target.GetComponent<Health>();
                    if (health != null)
                    {
                        float targetHealth = health.GetMaxHealth();
                        if (targetHealth > highestHealth)
                        {
                            highestHealth = targetHealth;
                            strongestTarget = target;
                        }
                    }
                }
            }

            if (strongestTarget != null && distanceToEnemy <= range)
            {
                return strongestTarget;
            }
        }
        return null;
    }

    static Transform FindWeakestEnemy(List<Transform> availableTargets, Transform shooter, float range)
    {
        if (availableTargets != null && availableTargets.Count > 0)
        {
            float lowestHealth = Mathf.Infinity;
            Transform weakestTarget = null;
            float distanceToEnemy = 0;
            foreach (Transform target in availableTargets)
            {
                if (target != null)
                {
                    distanceToEnemy = Vector3.Distance(shooter.position, target.transform.position);

                    Health health = target.GetComponent<Health>();
                    if (health != null)
                    {
                        float targetHealth = health.GetMaxHealth();
                        if (targetHealth < lowestHealth)
                        {
                            lowestHealth = targetHealth;
                            weakestTarget = target;
                        }
                    }
                }
            }

            if (weakestTarget != null && distanceToEnemy <= range)
            {
                return weakestTarget;
            }
        }
        return null;
    }

    static Transform FindRandomEnemy(List<Transform> possibleTargets, Transform shooter, float range)
    {
        if (possibleTargets != null && possibleTargets.Count > 0)
        {
            float distanceToEnemy = Mathf.Infinity;
            Transform randomTarget = null;

            int i = 0;
            while (distanceToEnemy > range && i < randomCheckLimit)
            {
                if (possibleTargets.Count > 0)
                {
                    randomTarget = possibleTargets[Random.Range(0, possibleTargets.Count)];
                    if (randomTarget)
                    {
                        distanceToEnemy = Vector3.Distance(shooter.position, randomTarget.transform.position);
                    }
                    if (distanceToEnemy > range)
                    {
                        possibleTargets.Remove(randomTarget);
                    }

                    i++;
                }
                else
                {
                    break;
                }

            }


            return randomTarget;
        }
        return null;
    }

    // Find the nearest target within range
    static Transform FindNearestEnemy(List<Transform> possibleTargets, Transform shooter, float colliderRadius)
    {

        if (possibleTargets != null && possibleTargets.Count > 0)
        {
            float shortestDistance = Mathf.Infinity;
            Transform nearestEnemy = null;

            foreach (Transform enemy in possibleTargets)
            {
                if (enemy != null)
                {
                    float distanceToEnemy = Vector3.Distance(shooter.position, enemy.transform.position);
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    }
                }

            }

            if (nearestEnemy != null && shortestDistance <= colliderRadius)
            {
                return nearestEnemy;
            }
        }

        return null;

    }
}
