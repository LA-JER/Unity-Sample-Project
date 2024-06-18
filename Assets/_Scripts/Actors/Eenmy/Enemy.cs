using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IHoverInfo
{
    public delegate void EnemyDeathHandler(GameObject killer, EnemyRank rank, int value);
    public static event EnemyDeathHandler OnEnemyKilled;
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
    public  List<Transform> waypoints = new List<Transform>();

    public Rigidbody2D rb;
    private Health health;
    public  StatManager statManager;
    private float deathLingerDuration = .25f;

    private void Awake()
    {
        waypoints = EnemyPathManager.Instance.GetWayPoints();
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();
    }

    
    public abstract void Move();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null )
        {
            if(collision.CompareTag("Home")) 
            {
                OnBaseCollide(collision);

            } else if(collision.CompareTag("Turret"))
            {
                
                    OnTurretCollide(collision);
                
            } else if(collision.CompareTag("Enemy"))
            {
                OnEnemyCollide(collision);
            }

        }
    }

    public abstract void OnBaseCollide(Collider2D collision);
    public abstract void OnEnemyCollide(Collider2D collision);
    public abstract void OnTurretCollide(Collider2D collision);

    void Initialize()
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
        health.onHealthZero += OnHealthZero;
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

    public abstract void OnDeath(GameObject killer);
     void OnHealthZero(GameObject source)
    {
        //Debugger.Log(Debugger.AlertType.Verbose, $"{name} killed by {source.name}");
        OnDeath(source);
        OnEnemyKilled?.Invoke(source, rank, value);
        //this.enabled = false;
    }

    private void OnDestroy()
    {
        health.onHealthZero -= OnHealthZero;
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
