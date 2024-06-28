using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathCondition : ItemCondition
{
    public Enemy.EnemyRank rankNeeded = Enemy.EnemyRank.Minor;
    [Tooltip("If false, allows this item to be triggered from the chosen rankNeeded and higher")]
    public bool strictTyping = true;
    public int killsNeeded = 1;

    public int kills = 0;

    // Start is called before the first frame update
    void Start()
    {
        Enemy.OnEnemyKilled += Enemy_OnEnemyKilled;
    }

    private void Enemy_OnEnemyKilled(GameObject killer, Enemy.EnemyRank rank, int value)
    {
        if(strictTyping)
        {
            if(rank == rankNeeded)
            {
                kills++;
            }
        } else
        {
            if((int)rank >= (int)rankNeeded) 
            {
                kills++;
            }
        }

        if(kills >= killsNeeded)
        {
            Trigger();
            kills = 0;
        }
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyKilled -= Enemy_OnEnemyKilled;
    }

    public override void TransferOwnership(GameObject newOwner)
    {
        kills = 0;
    }
}
