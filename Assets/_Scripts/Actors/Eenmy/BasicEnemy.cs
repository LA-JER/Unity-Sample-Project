using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : Enemy
{


    private float distanceThreshold = 0.05f;

    //moves toward the first Transform if the List "Targets",
    //when it reaches its target, begins moving towards the next target
    public override void Move()
    {
        if (waypoints.Count > 0 && rb != null)
        {
            Vector2 direction = (waypoints[0].position - transform.position).normalized;
            //Vector2 direction = (new Vector2(target.position.x, target.position.y) - GetRigidbody2D().position).normalized;
            rb.velocity = (direction * statManager.GetStat(StatManager.Stat.moveSpeed));
            //GetRigidbody2D().MovePosition(GetRigidbody2D().position *  direction);
            //check if the object is close enough to the waypoint
            if (Vector2.Distance(transform.position, waypoints[0].position) <= distanceThreshold)
            {

                waypoints.RemoveAt(0);
            }
        }
    }

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

        Destroy(gameObject, 0.25f);
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
