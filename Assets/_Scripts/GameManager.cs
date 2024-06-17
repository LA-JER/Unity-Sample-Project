using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Debugger.PrintType printType;
    [SerializeField] private Transform turretHolder;
    [SerializeField] private Transform projectileHolder;
    [SerializeField] private Transform enemyHolder;
    [SerializeField] private float goldChance = 0.5f;
    [SerializeField] private float refundPercent = 0.5f;

    private List<Turret> turrets = new List<Turret>();
    private float elapsedTime = 0;
    private int kills = 0;
    private int currencySpent = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
        Debugger.print = printType;

        HomeBase.OnBaseDie += GameOver;
        Shop.OnBuyTurret += Shop_OnBuyTurret;
        Enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
    }

    private void Enemy_OnEnemyDeath(GameObject souce, Enemy.EnemyRank rank, int value)
    {
        kills++;
    }

    private void Shop_OnBuyTurret(Turret turret, int c)
    {
        currencySpent += c;
        if (turret == null) return;
        turrets.Add(turret);

    }

    private void GameOver()
    {
        float totalDamage = 0;
        foreach (Turret turret in turrets)
        {
            totalDamage += turret.GetTotalDamage();
        }

        EndScreen.instance.ShowScreen("Game Over!", kills, totalDamage, currencySpent, elapsedTime);
        Time.timeScale = 0.25f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
    }


    private void OnDestroy()
    {
        HomeBase.OnBaseDie -= GameOver;
        Shop.OnBuyTurret -= Shop_OnBuyTurret;
        Enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;
    }

    public float GetRefundPercent()
    {
        return refundPercent;
    }
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public float GetGoldChance()
    {
        return goldChance;
    }
    public Transform GetTurretHolder()
    {
        return turretHolder;
    }
    public Transform GetProjectileHolder()
    {
        return projectileHolder;
    }
    public Transform GetEnemyHolder()
    {
        return enemyHolder;
    }
}
