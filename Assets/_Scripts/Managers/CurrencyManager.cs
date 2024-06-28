using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public delegate void CurrencyChange(int newTotal);
    public static event CurrencyChange onTotalChange;
    public static event CurrencyChange onRefund;
    public static event CurrencyChange onPurchase;

    public static CurrencyManager instance;

    [SerializeField] private int totalMoney;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        Enemy.OnEnemyKilled += Enemy_OnEnemyDeath;
    }

    private void Enemy_OnEnemyDeath(GameObject souce, Enemy.EnemyRank rank, int value)
    {
        float roll = Random.value;

        if(roll <= GameManager.Instance.GetGoldChance())
        {

            totalMoney += value;
            onTotalChange?.Invoke(totalMoney);
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        onTotalChange?.Invoke(totalMoney);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Buy(int amount)
    {
        if(totalMoney >= amount)
        {
            totalMoney -= amount;
            onTotalChange?.Invoke(totalMoney);
            onPurchase?.Invoke(amount);
        }
    }

    public void Refund(int amount)
    {
        totalMoney += amount;
        onTotalChange?.Invoke(totalMoney);
        onRefund?.Invoke(amount);
    }

    public bool CanBuy(int amount)
    {
        return totalMoney >= amount;
    }


    private void OnDestroy()
    {
        Enemy.OnEnemyKilled -= Enemy_OnEnemyDeath;
    }
}
