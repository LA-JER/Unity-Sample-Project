using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public class Projectile : DamageElement
{
    private Transform target;
    private StatManager statManager;
    public List<GameObject> effects = new List<GameObject>();
    private IProjectileMovement projectileMovement;
    private string enemyTag = "Enemy";
    private int pierced = 0;

    private void Awake()
    {
        projectileMovement = GetComponent<IProjectileMovement>();
        statManager = GetComponent<StatManager>();
        if (statManager == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached statManager! Did you forget to add the component?");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
         
        // Destroy the projectile after 'lifespan' seconds
        Destroy(gameObject, statManager.GetStat(Stat.lifeSpan));
    }

    // Update is called once per frame
    void Update()
    {
        if (projectileMovement != null)
        {
            projectileMovement.MoveProjectile();
        }
        // Move the projectile forward
        else
        {
            transform.Translate(Vector2.up * statManager.GetStat(Stat.projectileSpeed) * Time.deltaTime);
        }
    }


    public override void Initialize(GameObject source, StatManager other, bool setSizeAtStart = false)
    {
        //Debugger.Log(Debugger.AlertType.Info, $"Projectile.Initialize called with statManager {statManager} and other statManager: {other}");
        if (other != null)
        {
            statManager = GetComponent<StatManager>();
            if (statManager != null)
            {
                statManager.UpdateApplicableStatsFrom(other);
                //Changes this object's size based on the stats sizeX and sizeY
                if(setSizeAtStart )
                {
                    transform.localScale *= new Vector2(statManager.GetStat(Stat.sizeX), statManager.GetStat(Stat.sizeY));
                }
                
            }
        }
        if (source != null)
        {
            this.source = source;
        }
    }

    // If the projectile collides with an enemy, deal damage and destroy the projectile unless
    // the projectile can pierce more targets
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && !collision.isTrigger)
        {
            if (collision.CompareTag(enemyTag))
            {
                // Get the maxHealth component of the enemy
                Health health = collision.GetComponent<Health>();

                // If the enemy has Health component, deal damage
                if (health != null)
                {
                    //deals damage to the health component
                    DamageInstance dmg = DealDamage(source, statManager, health);

                    DoEffects(health.gameObject);

                    pierced++;

                    if (dmg.isCritical && pierced <= statManager.GetStat(Stat.critBounce))
                    {
                        return;
                    }

                    if (pierced > statManager.GetStat(Stat.pierceCount))
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    public void AddEffect(GameObject effect)
    {
        if(effect != null)
        {
            if (!effects.Contains(effect))
            {
                effects.Add(effect);
            }
        }
    }


    private void DoEffects(GameObject hit)
    {
        foreach(GameObject effect in effects)
        {
            ProjectileHitEffect hitEffect = effect.GetComponent<ProjectileHitEffect>();
            if (hitEffect != null)
            {
                hitEffect.DoThing(this, hit);
            }
            
        }
    }


    public Transform GetTarget()
    {
        return target;
    }

    public string GetEnemyTag()
    {
        return enemyTag;
    }
}
