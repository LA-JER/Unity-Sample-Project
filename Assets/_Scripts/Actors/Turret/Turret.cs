using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Targeting;
using static StatManager;
using AYellowpaper.SerializedCollections;
using static ITargetable;

public abstract class Turret : MonoBehaviour, IHoverInfo, IPurchaseAble
{
    public delegate void TurretActivate();
    public event TurretActivate OnActivate;
    public event TurretActivate OnDeactivate;
    public event TurretActivate OnTurretShoot;

    public delegate void TurretReload(Vector2 position, float duration);
    public static event TurretReload OnReload;

    [SerializeField] private Sprite Icon;

    [SerializeField] private string ActorName;

    [SerializeField] private string Description;
    public bool ignoresReload = false;
    public TargetingStyle targetingStyle = TargetingStyle.closest;
    public PlayArea playArea = PlayArea.Ground;
    [SerializedDictionary("Enemy Type", "Can Target")]
    public SerializedDictionary<PlayArea, bool> targetsCapable= new SerializedDictionary<PlayArea, bool>();
    [SerializeField] private int price = 3;
    [SerializeField] private string enemyTag = "Enemy"; // Tag of the target
    public Transform pivot; 
    public GameObject projectilePrefab; 
    public Transform mainFirePoint; // Point from where projectiles are fired
    [SerializeField] private CircleCollider2D rangeCollider;

    public StatManager statManager;
    public DamageElementManager projectileManager;
    private List<Transform> targetsInRange = new List<Transform>(); // keeps track of enemies in range of this turret
    private float fireCountdown = 0f; // Countdown timer for next shot
    public float timeBetweenConsecutiveShot = 0.1f;
    private bool isActivated = false;
    private float totalDamageDealt = 0;
    private int totalKills = 0;
    private int totalSpent = 0;
    private int shots;
    private float reloadCountdown = 0f;

    private void Awake()
    {
        DamageElement.OnDamageDealt += DamageDealer_OnDamageDealt;
        Enemy.OnEnemyKilled += Enemy_OnEnemyDeath;
        UpgradeDisplay.OnBuyUpgrade += UpgradeDisplay_OnBuyUpgrade;
        //GameManager.OnPaused += GameManager_OnPaused;
        totalSpent += price;
    }


    private void UpgradeDisplay_OnBuyUpgrade(GameObject owner, UpgradeNode chosenUpgrade)
    {
        if(owner != null)
        {
            if(owner == gameObject)
            {
                if(chosenUpgrade != null)
                {
                    totalSpent += chosenUpgrade.price;
                }
            }
        }
    }

    private void Enemy_OnEnemyDeath(GameObject killer, Enemy.EnemyRank rank, int value)
    {
        if (killer != null) 
        {

            if (killer == gameObject)
            {
                totalKills++;
            }
        }
        
    }

    private void DamageDealer_OnDamageDealt(GameObject source, GameObject hit, DamageInstance damage)
    {
        if(source == null) return;
        if(source == gameObject)
        {
            totalDamageDealt += damage.damage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (isActivated && GameManager.Instance.IsGamePaused() == false)
        {
            AimAndFire();
        }
        
    }


     void AimAndFire()
    {
        if (statManager == null) return;
        
        if (rangeCollider != null)
        {
            rangeCollider.radius = statManager.GetStat(Stat.range);
        }

        int targetCount = 1;
        if (statManager.HasStat(Stat.targetMax))
        {
            targetCount = (int)statManager.GetStat(Stat.targetMax); ;
        }
        
        List<Transform> targets = TargetingChip(targetsInRange, targetingStyle, targetCount, transform, rangeCollider.radius);
        if (targets != null && targets.Count > 0 && targets[0] != null)
        {
            RotateAim(targets[0]);

            float fireRate = statManager.GetStat(Stat.fireRate);

            //special case if fireRate was reduced to zero
            if ( fireRate <= 0)
            {
                return;
            }


            if (ignoresReload == false && shots >= statManager.GetStat(Stat.magSize))
            {
                shots = 0;
                reloadCountdown = statManager.GetStat(Stat.reloadTime);
                OnReload?.Invoke(transform.position, reloadCountdown);
            }

            // Fire at the targetsInRange if countdown reaches zero
            if (fireCountdown <= 0f && reloadCountdown <= 0f)
            {
                shots++;
                OnTurretShoot?.Invoke();
                StartCoroutine(Shoot(targets));
                fireCountdown = 1f / fireRate;
            }

            fireCountdown -= Time.deltaTime;
            reloadCountdown -= Time.deltaTime;
        }
        
    }

    //called in update
    public virtual void RotateAim(Transform target)
    {
        if (target != null)
        {
            float rotationSpeed = statManager.GetStat(StatManager.Stat.rotateSpeed);


            // Get the direction to the targetsInRange from the barrel's position
            Vector3 direction = (target.position - pivot.position).normalized;

            // Calculate the angle in degrees
            float angle = Mathf.Atan2(direction.y, direction.x);


            float angleDegrees = angle * Mathf.Rad2Deg + 270f;
            //float angleDeviatedDegrees = angleDeviated * Mathf.Rad2Deg + 270f;

            // Smoothly rotate the actor toward the targetsInRange
            pivot.rotation = Quaternion.Slerp(pivot.rotation, Quaternion.Euler(0, 0, angleDegrees), rotationSpeed * Time.deltaTime);
            //pivot2.rotation = Quaternion.Slerp(pivot.rotation, Quaternion.Euler(0, 0, angleDeviatedDegrees), rotationSpeed * Time.deltaTime);
        }
    }


    public abstract IEnumerator Shoot(List<Transform> targets);

    // Handles enemy entering tower's range
    private void OnTriggerStay2D(Collider2D collision)
    {
        EliminateNulls();
        if (collision == null) return;
        if (collision.CompareTag(enemyTag))
        {
            ITargetable targetable = collision.GetComponent<ITargetable>();
            if(targetable != null)
            {
                if ( targetable.IsTargetable() && CanTarget(targetable.GetPlayableArea()) )
                {
                    if (targetsInRange.Contains(collision.transform) == false)
                    {
                        targetsInRange.Add(collision.transform);
                    }
                }
                
            }
            


        }
    }

    // Handle enemy leaving tower's range
    private void OnTriggerExit2D(Collider2D collision)
    {
        EliminateNulls();
        if (collision == null) return;
        if (collision.CompareTag(enemyTag))
        {
            ITargetable targetable = collision.GetComponent<ITargetable>();
            if (targetable != null)
            {
                if (targetable.IsTargetable() && CanTarget(targetable.GetPlayableArea()) )
                {
                    if (targetsInRange.Contains(collision.transform) )
                    {
                        targetsInRange.Remove(collision.transform);
                    }
                }

            }
        }
    }

    bool CanTarget(PlayArea otherArea)
    {
        if(targetsCapable == null || !targetsCapable.ContainsKey(otherArea))
        {
            Debugger.Log(Debugger.AlertType.Error, $"Did not have {otherArea} in dictionary! Did you forget to add it to the turret?");
            return false;
        }

        return targetsCapable[otherArea];
    }

    private void Initialize()
    {
        statManager = GetComponent<StatManager>();
        if (statManager == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached statManager! Did you forget to add the component?");
        }
        projectileManager = GetComponent<DamageElementManager>();
        if(projectileManager == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} coud not find its attached projectileManager! Did you forget to add the component?");
        }
        if (rangeCollider == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name}'s range collider was not defined! Did you forget to add the component?");
        }

        if (statManager != null && rangeCollider != null)
        {
            float range = statManager.GetStat(StatManager.Stat.range);
            rangeCollider.radius = range;
            
        }

        
        if (!statManager.HasStat(Stat.targetMax))
        {
            statManager.AddStatDefault(Stat.targetMax);
        }
        if (!statManager.HasStat(Stat.reloadTime))
        {
            statManager.AddStatDefault(Stat.reloadTime);
        }
        if (!statManager.HasStat(Stat.magSize))
        {
            statManager.AddStatDefault(Stat.magSize);
        }
    }

    private void OnDestroy()
    {
        DamageElement.OnDamageDealt -= DamageDealer_OnDamageDealt;
        Enemy.OnEnemyKilled -= Enemy_OnEnemyDeath;
        UpgradeDisplay.OnBuyUpgrade -= UpgradeDisplay_OnBuyUpgrade;
        //GameManager.OnPaused -= GameManager_OnPaused;
    }

    public Dictionary<PlayArea, bool> GetCapabilities()
    {
        Dictionary<PlayArea, bool> dict = new Dictionary<PlayArea, bool>();
        foreach(var cap in targetsCapable)
        {
            dict.Add(cap.Key, cap.Value);
        }
        return dict;
    }

    private void EliminateNulls()
    {
        targetsInRange.RemoveAll(item => item == null);
    }
    
    public int GetTotalSpent()
    {
        return totalSpent;
    }

    public float GetTotalDamage()
    {
        return totalDamageDealt;
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    public int GetKills()
    {
        return totalKills;
    }

    public int GetPrice()
    {
        return price;
    }

    public void Activate(GameObject owner)
    {
        OnActivate?.Invoke();
        isActivated = true;
    }

    public void Deactivate(GameObject owner)
    {
        OnDeactivate?.Invoke();
        isActivated = false;
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
