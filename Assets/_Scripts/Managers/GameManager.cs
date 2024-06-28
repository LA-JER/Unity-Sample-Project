using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public delegate void OnGame(bool won);
    public static event OnGame OnGameEnd;

    public delegate void OnPause(bool isPaused);
    public static event OnPause OnPaused;

    public static GameManager Instance;

    public Debugger.PrintType printType;
    [SerializeField] private Transform turretHolder;
    [SerializeField] private Transform projectileHolder;
    [SerializeField] private Transform enemyHolder;
    [SerializeField] private float goldChance = 0.5f;
    [SerializeField] private float refundPercent = 0.5f;

    private List<Turret> turrets = new List<Turret>();
    public float elapsedTime = 0;
    private int kills = 0;
    private int currencySpent = 0;
    private bool gameDone = false;
    private bool isGamePaused = false;
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
        CurrencyManager.onPurchase += CurrencyManager_onPurchase;
        CurrencyManager.onRefund += CurrencyManager_onRefund;
        Enemy.OnEnemyKilled += Enemy_OnEnemyDeath;
        WaveManager.OnAllEnemiesKilled += GameWon;
        AlertManager.OnAlert += Pause;
        PauseMenu.onWorldStop += Pause;

        StartCoroutine(RealTimeElapsed());
    }

    private void CurrencyManager_onRefund(int refund)
    {
        currencySpent -= refund;
    }

    private void CurrencyManager_onPurchase(int purchase)
    {
        currencySpent += purchase;
    }

    private void Pause(bool isShown)
    {
      
        OnPaused?.Invoke(isShown);
        isGamePaused = isShown;
       

    }

    private void Enemy_OnEnemyDeath(GameObject souce, Enemy.EnemyRank rank, int value)
    {
        kills++;
    }


    void GameWon()
    {
        OnGameEnd?.Invoke(true);
        GameDone("You Win!");
        DataHandler.IncrementFlag(DataHandler.Flag.totalWins);
    }

   private void GameOver()
    {
        OnGameEnd?.Invoke(false);
        GameDone("Game Over!");
        
    }


    private void GameDone(string message)
    {
        if (gameDone == false)
        {
            float totalDamage = 0;
            foreach (Turret turret in turrets)
            {
                totalDamage += turret.GetTotalDamage();
            }

            EndScreen.instance.ShowScreen(message, kills, totalDamage, currencySpent, elapsedTime);

            DataHandler.IncrementFlag(DataHandler.Flag.totalDamage,(int) totalDamage);
            DataHandler.IncrementFlag(DataHandler.Flag.totalKills, kills);
            DataHandler.IncrementFlag(DataHandler.Flag.totalSpent, currencySpent);
            DataHandler.IncrementFlag(DataHandler.Flag.totalTime, (int)elapsedTime);

            Time.timeScale = 0.25f;
            gameDone = true;
        }
        
    }


    private IEnumerator RealTimeElapsed()
    {
        while(gameDone == false)
        {
            float adjustedWait = Time.timeScale;

            yield return new WaitForSeconds(adjustedWait);
            elapsedTime++;
        }
        
        
    }

    private void OnDestroy()
    {
        HomeBase.OnBaseDie -= GameOver;
        CurrencyManager.onPurchase -= CurrencyManager_onPurchase;
        CurrencyManager.onRefund -= CurrencyManager_onRefund;
        Enemy.OnEnemyKilled -= Enemy_OnEnemyDeath;
        WaveManager.OnAllEnemiesKilled -= GameWon;
        AlertManager.OnAlert -= Pause;
        PauseMenu.onWorldStop -= Pause;
    }

    public bool IsGamePaused()
    {
        return isGamePaused;
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
