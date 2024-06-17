using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHoverInfo
{
    public delegate void EnemyDeathHandler(GameObject killer, EnemyRank rank, int value);
    public static event EnemyDeathHandler OnEnemyDeath;
    public enum EnemyRank
    {
        Minor,
        Major,
        Boss
    }

    [SerializeField] private Sprite Icon;

    [SerializeField] private string ActorName;

    [SerializeField] private string Description;
    [SerializeField] private EnemyRank rank;
    [SerializeField] private int value;
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    private Rigidbody2D rb;
    private Health health;
    private StatManager statManager;
    private float distanceThreshold = 0.35f;

    private void Awake()
    {
        waypoints = EnemyPathManager.Instance.GetWayPoints();
    }

    //moves toward the first Transform if the List "Targets",
    //when it reaches its target, begins moving towards the next target
    public virtual void Move()
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

    public virtual void Attack(Health other)
    {
        if(other != null)
        {
            other.Damage(gameObject, statManager.GetStat(StatManager.Stat.damage), false);
        }
    }

     public void Initialize()
    {
        statManager = GetComponent<StatManager>();
        if (statManager == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached StatManager! Did you forget to add the component?");
        }
        rb = GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached RigidBody2D! Did you forget to add the component?");

        }
        health = GetComponent<Health>();
        if(health == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached Health! Did you forget to add the component?");
        }
        health.onHealthZero += onDeath;
    }

    public void AddTarget(Transform target)
    {
        waypoints.Add(target);
    }

    public void AddTargets(List<Transform> newTargets)
    {
        foreach(Transform t in newTargets)
        {
            waypoints.Add(t);
        }
    }


    public virtual void onDeath(GameObject source)
    {
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} killed by {source.name}");
        OnEnemyDeath?.Invoke(source, rank, value);
        Destroy(gameObject);
    }

    public string GetName()
    {
        return ActorName;
    }

    public string GetDescription()
    {
        return Description;
    }

    public Sprite GetIcon()
    {
        return Icon;
    }
}
