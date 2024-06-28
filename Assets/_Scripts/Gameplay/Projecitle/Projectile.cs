using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ITargetable;
using static Projectile;
using static StatManager;

public class Projectile : DamageElement
{
    public delegate void ProjectileHit(GameObject hit, DamageInstance damage);
    public event ProjectileHit OnProjectileHit;

    private Transform target;
    private StatManager statManager;

    
    public Collider2D hurtBox;
    public ProjectileMovement projectileMovement;
    private string enemyTag = "Enemy";
    private int pierced = 0;
    public Rigidbody2D rb;
    public float hurtSameEnemyCooldown = 2f;

    private List<GameObject> hitObjects = new List<GameObject>();
    private void Awake()
    {
        //projectileMovement = GetComponent<ProjectileMovement>();
        rb = GetComponent<Rigidbody2D>();
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
        // Move the projectile forward
        if (projectileMovement == null)
        {
            transform.Translate(Vector2.up * statManager.GetStat(Stat.projectileSpeed) * Time.deltaTime);
        }

        if (projectileMovement != null && !projectileMovement.usesFixedUpdate)
        {
            projectileMovement.MoveProjectile();
        }


    }

    private void FixedUpdate()
    {
        if (projectileMovement != null && projectileMovement.usesFixedUpdate)
        {
            projectileMovement.MoveProjectile();
        }
    }

    public override void Initialize(GameObject source, StatManager other, Dictionary<PlayArea, bool> targetsCapable, bool setSizeAtStart = false)
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
        if(targetsCapable != null)
        {
            foreach (var capable in targetsCapable)
            {
                if (this.targetsCapable.ContainsKey(capable.Key))
                {
                    this.targetsCapable[capable.Key] = capable.Value;
                }
            }
        }
    }
    // If the projectile collides with an enemy, deal damage and destroy the projectile unless
    // the projectile can pierce more targets
    public void OnHurtBoxEntered(Collider2D collision)
    {
        if (collision == null) return;

        if (!collision.CompareTag(enemyTag)) return;

        //skip if we recently hit the same object
        if (hitObjects.Contains(collision.gameObject)) return;

        //get the targetable component
        ITargetable targetable = collision.GetComponent<ITargetable>();
        if (targetable == null) return;

        if(CanTarget(targetable.GetPlayableArea()) == false) return;
        

        //deals damage to the health component
        DamageInstance dmg = DealDamage(source, statManager, collision.gameObject);
        OnProjectileHit?.Invoke(collision.gameObject, dmg);
        DoEffects(collision.gameObject);

        pierced++;

        StartCoroutine(CoolDown(collision.gameObject));

        if (dmg.isCritical && pierced <= statManager.GetStat(Stat.critBounce))
        {
            return;
        }

        if (pierced > statManager.GetStat(Stat.pierceCount))
        {
            Destroy(gameObject);
        }
                
        
        
    }


 

    IEnumerator CoolDown(GameObject hit)
    {
        hitObjects.Add(hit);
        yield return new WaitForSeconds(hurtSameEnemyCooldown);
        hitObjects.Remove(hit);
    }


    public float GetStat(Stat stat)
    {
        return statManager.GetStat(stat);
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
