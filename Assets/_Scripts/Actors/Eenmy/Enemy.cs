using System.Collections.Generic;
using UnityEngine;
using static ITargetable;

public abstract class Enemy : MonoBehaviour, IHoverInfo, ITargetable
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
    [SerializeField] private PlayArea playArea;
    [SerializeField] private int value;
    public  List<Vector3> waypoints = new List<Vector3>();

    
    public Rigidbody2D rb;
    private Health health;
    public  StatManager statManager;
    public float deathLingerDuration = .25f;
    public float  distanceThreshold = 0.05f;
    private bool setsTargetsItself = true;
    private bool isTargetable = true;
    //private bool isPaused = false;

    private void Start()
    {
        
        Initialize();
    }

    /*
    private void GameManager_OnPaused(bool paused)
    {
        isPaused = paused;
    }*/

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!GameManager.Instance.IsGamePaused())
        {
            Move();
        } else
        {
            rb.velocity = Vector3.zero;
        }
        
    }

    //moves toward the first Transform if the List "Targets",
    //when it reaches its target, begins moving towards the next target

    public virtual void Move()
    {
        if (waypoints.Count > 0 && rb != null)
        {
            Vector2 direction = (waypoints[0] - transform.position).normalized;
            //Vector2 direction = (new Vector2(target.position.x, target.position.y) - GetRigidbody2D().position).normalized;
            rb.velocity = (direction * statManager.GetStat(StatManager.Stat.moveSpeed));
            //GetRigidbody2D().MovePosition(GetRigidbody2D().position *  direction);
            //check if the object is close enough to the waypoint
            if (Vector2.Distance(transform.position, waypoints[0]) <= distanceThreshold)
            {

                waypoints.RemoveAt(0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null )
        {
            if(!GameManager.Instance.IsGamePaused())
            {
                if (collision.CompareTag("Home"))
                {
                    OnBaseCollide(collision);

                }
                else if (collision.CompareTag("Turret"))
                {

                    OnTurretCollide(collision);

                }
                else if (collision.CompareTag("Enemy"))
                {
                    OnEnemyCollide(collision);
                }

                if (collision.CompareTag("TargetingBlocker"))
                {
                    isTargetable = false;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision != null)
        {
            if (collision.CompareTag("TargetingBlocker"))
            {
                isTargetable = true;
            }
        }
    }

    public abstract void OnBaseCollide(Collider2D collision);
    public abstract void OnEnemyCollide(Collider2D collision);
    public abstract void OnTurretCollide(Collider2D collision);

    void Initialize()
    {
        if(setsTargetsItself)
        {
            waypoints = EnemyPathManager.Instance.Getpath();
        }

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
        //GameManager.OnPaused += GameManager_OnPaused;
    }

    public bool IsTargetable()
    {
        return isTargetable;
    }

    public void SetTargetsItself(bool setsItself)
    {
        setsTargetsItself = setsItself;
    }


    public void SetPath(List<Transform> newTargets)
    {
        foreach(Transform t in newTargets)
        {
            waypoints.Add(t.position);
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
        //GameManager.OnPaused -= GameManager_OnPaused;
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

    public PlayArea GetPlayableArea()
    {
        return playArea;
    }
}
