using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;
using static Targeting;

public class CritBounceMovement : ProjectileMovement
{
    [SerializeField] CircleCollider2D rangeCollider;
    [SerializeField] private TargetingStyle targetingStyle = TargetingStyle.random;
    [Tooltip("Minimum distance from current hit to be considered a new valid target. Inclusive")]
    [SerializeField] private float margin = .1f;
    //public float angleOffest = 270f;


    private void Start()
    {
        if (projectile != null)
        {
            if (rangeCollider != null)
            {
                rangeCollider.radius = projectile.GetStat(Stat.projectileTargetRadius);
            }
            projectile.OnProjectileHit += Projectile_OnDamage;
        }
    }

    public override void MoveProjectile()
    {
        if (projectile == null) { return; }
        projectile.transform.Translate(Vector2.up * projectile.GetStat(Stat.projectileSpeed)* Time.deltaTime);

    }

    //When the source projectile deals critical damage
    //begin moving towards a new random enemy
    private void Projectile_OnDamage(GameObject hit, DamageInstance damage)
    {
        if (damage.isCritical)
        {
            //make sure we dont hit the same enemy twice
            if (targets.Contains(hit.transform))
            {
                targets.Remove(hit.transform);
            }

            List<Transform> validTargets = new List<Transform>();
            //tagets that are too close are not valid
            foreach(Transform inRange in targets)
            {
                if(inRange == null) continue;
                float distance = Vector2.Distance(inRange.position, transform.position);
                if(distance > margin)
                {
                   validTargets.Add(inRange);
                }
            }

            //get 1 target based on chosen targeting style
            List<Transform> target = TargetingChip(validTargets, targetingStyle, 1, transform, rangeCollider.radius);
            if (target != null && target.Count > 0)
            {
                Vector2 direction = (target[0].position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x);


                float angleDegrees = angle * Mathf.Rad2Deg + 270f;

                // Face the new enemy
                projectile.transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
            }
        }
    }

}
