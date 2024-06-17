using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public class Explosion : DamageDealer
{
    public string targetTag = "Enemy";

    private StatManager statManager;
    private Vector3 initialScale;
   // private float waitInterval = .125f;

    private List<Health> damagedList = new List<Health>();
    //private bool isScaling = false;
    // Start is called before the first frame update
    void Start()
    {
        statManager = GetComponent<StatManager>();
        initialScale = transform.localScale;
        StartCoroutine(ScaleOverTime());
        Destroy(gameObject, statManager.GetStat(StatManager.Stat.lifeSpan ));
    }

    public override void Initialize(GameObject source, StatManager other, bool setSizeAtStart = false)
    {
        //Debugger.Log(Debugger.AlertType.Info, $"Projectile.Initialize called with statManager {statManager} and other statManager: {other}");
        if (other != null)
        {
            statManager = GetComponent<StatManager>();
            if (statManager != null)
            {
                statManager.UpdateApplicableStatsFrom(other);
                //Changes this object's size based on the stats sizeX and sizeY
                if (setSizeAtStart)
                {
                    transform.localScale *= new Vector2(statManager.GetStat(Stat.sizeX), statManager.GetStat(Stat.sizeY));
                }

            }
        }
        if (source != null)
        {
            this.source = source;
        }
    }

    private IEnumerator ScaleOverTime()
    {
        //isScaling = true;
        float elapsedTime = 0f;

        while (elapsedTime < statManager.GetStat(StatManager.Stat.lifeSpan))
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / statManager.GetStat(StatManager.Stat.lifeSpan);

            float sizeX = statManager.GetStat(StatManager.Stat.sizeX);
            float sizeY = statManager.GetStat(StatManager.Stat.sizeY);

            transform.localScale = Vector3.Lerp(initialScale, new Vector2(sizeX, sizeY), 1/t);
            yield return new WaitForEndOfFrame();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null)
        {
            if(collision.CompareTag(targetTag))
            {
                Health health = collision.GetComponent<Health>();
                if(health != null)
                {
                    //ensure we only damage enemies once
                    //prevents enemies that leave and enter the explosion quickly from getting hurt twice
                    if (!damagedList.Contains(health))
                    {
                        DamageInstance dmg = DealDamage(source, statManager, health);
                        damagedList.Add(health);
                    }
                }
            }
        }
    }

}
