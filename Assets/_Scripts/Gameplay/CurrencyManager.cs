using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public delegate void CurrencyChange(int newValue);
    public static event CurrencyChange onCurrencyChange;

    public static CurrencyManager instance;

    [SerializeField] private int currencyAmount;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        Enemy.OnEnemyDeath += Enemy_OnEnemyDeath;
    }

    private void Enemy_OnEnemyDeath(GameObject souce, Enemy.EnemyRank rank, int value)
    {
        float roll = Random.value;

        if(roll <= GameManager.Instance.GetGoldChance())
        {

            currencyAmount += value;
            onCurrencyChange?.Invoke(currencyAmount);
        }        
    }

    // Start is called before the first frame update
    void Start()
    {
        onCurrencyChange?.Invoke(currencyAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Buy(int amount)
    {
        if(currencyAmount >= amount)
        {
            currencyAmount -= amount;
            onCurrencyChange?.Invoke(currencyAmount);
        }
    }

    public void Refund(int amount)
    {
        currencyAmount += amount;
        onCurrencyChange?.Invoke(currencyAmount);
    }

    public bool CanBuy(int amount)
    {
        return currencyAmount >= amount;
    }


    private void OnDestroy()
    {
        Enemy.OnEnemyDeath -= Enemy_OnEnemyDeath;
    }
}
