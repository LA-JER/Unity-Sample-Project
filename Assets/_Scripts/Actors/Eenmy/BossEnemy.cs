using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : Enemy
{
    [SerializeField] private GameObject babyprefab;
    [SerializeField] private int babyCount;
    [SerializeField] private Buff turretDebuff;

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

        for (int i = 0; i < babyCount; i++)
        {
            GameObject babyOBJ = Instantiate(babyprefab, GameManager.Instance.GetEnemyHolder());
            babyOBJ.transform.position = transform.position + new Vector3(Random.value, Random.value, 0);
            Enemy enemy = babyOBJ.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.SetTargetsItself(false);
                enemy.SetPath(EnemyPathManager.Instance.GetClosestWayPoints(babyOBJ.transform.position));
            }
        }

        enabled = false;
        Destroy(gameObject, deathLingerDuration);
        
        return;
    }

    public override void OnEnemyCollide(Collider2D collision)
    {
        //throw new System.NotImplementedException();
        return;
    }

    public override void OnTurretCollide(Collider2D collision)
    {
        //prevents triggering from their range radius
        if (!collision.isTrigger)
        {
            BuffManager buffManager = collision.GetComponent<BuffManager>();
            if(buffManager != null)
            {
                buffManager.ApplyTimedBuff(turretDebuff);
            }
        }
    }
}
