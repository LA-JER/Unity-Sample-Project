using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

public class MultiFirePointTurret : Turret
{
    [SerializeField] private List<Transform> allFirePoints = new List<Transform>();


    public override IEnumerator Shoot(List<Transform> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            float coneAngle = statManager.GetStat(StatManager.Stat.coneAngle);
            float fireCount = statManager.GetStat(StatManager.Stat.fireCount);

            for (int i = 0; i < fireCount; i++)
            {
                Transform point = allFirePoints[i % allFirePoints.Count];

                float coneDeviation = Random.Range(-coneAngle * 0.5f, coneAngle * 0.5f);
                Quaternion deviation = point.rotation * Quaternion.Euler(0, 0, coneDeviation);

                

                GameObject projectile = Instantiate(projectilePrefab, point.position, deviation, GameManager.Instance.GetProjectileHolder());
                Projectile projectile1 = projectile.GetComponent<Projectile>();
                if (projectile1 != null)
                {
                    projectile1.Initialize(gameObject, statManager, true);
                    projectileManager.ApplyEffects(projectile1);
                }
                else
                {
                    Debugger.Log(Debugger.AlertType.Warning, $"{name} tried to init. projectile but the script was not found!");
                }

                yield return new WaitForSeconds(timeBetweenConsecutiveShot);
            }
        }
    }
}
