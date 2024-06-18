using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{


    

    

    public override void OnBaseCollide(Collider2D collision)
    {
        Health health = collision.GetComponent<Health>();
        if (health != null)
        {
            health.Damage(gameObject, statManager.GetStat(StatManager.Stat.damage), false);
            OnDeath(null);
        }
    }

    public override void OnDeath(GameObject killer)
    {
        rb.velocity = Vector2.zero;

        Destroy(gameObject, deathLingerDuration);
        enabled = false;
        return;
    }

    public override void OnEnemyCollide(Collider2D collision)
    {
        return;
    }

    public override void OnTurretCollide(Collider2D collision)
    {
        return; 
    }
}
