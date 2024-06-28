using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    public Projectile projectile;
    public bool usesFixedUpdate = false;
    public abstract void MoveProjectile();

    public List<Transform> targets = new List<Transform>();
    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(projectile.GetEnemyTag()))
        {
            ITargetable targetable = collision.GetComponent<ITargetable>();
            if (targetable != null)
            {
                if (targetable.IsTargetable())
                {
                    if (!targets.Contains(collision.transform))
                    {
                        targets.Add(collision.transform);
                    }
                }

            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(projectile.GetEnemyTag()))
        {
            ITargetable targetable = collision.GetComponent<ITargetable>();
            if (targetable != null)
            {
                if (targetable.IsTargetable())
                {
                    if (targets.Contains(collision.transform))
                    {
                        targets.Remove(collision.transform);
                    }
                }

            }
        }
    }

}
