using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StatManager;

[CreateAssetMenu(menuName = "Tower Game/ Upgrade Node")]
public class UpgradeNode : Upgrade
{
    public TurretSpriteGroup spriteGroup;
    public List<UpgradeNode> nextUpgrades = new List<UpgradeNode>();
    public List<GameObject> hitEffects = new List<GameObject>();

    public override void ApplyUpgrade(GameObject owner)
    {
        if(owner != null)
        {
            BuffManager buffManager = owner.GetComponent<BuffManager>();
            if(buffManager != null)
            {
                
                foreach(var entry in statDictionary)
                {
                    Debugger.Log(Debugger.AlertType.Verbose, $"applying {entry.Value} to {entry.Key} on {owner}!");
                    buffManager.ApplyPermanentBuff(entry.Key, entry.Value.operation, entry.Value.modifier);
                }
                
            }

            ProjectileManager projectileManager = owner.GetComponent<ProjectileManager>();
            if(projectileManager != null)
            {
                foreach(GameObject effect in hitEffects)
                {
                    projectileManager.AddNewEffect(effect);
                }
            }

            SpriteManager spriteManager = owner.GetComponent<SpriteManager>();
            if(spriteManager != null)
            {
                spriteManager.ShowSprites(spriteGroup);
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} tried to apply upgrade but the given actor was null!");
        }
    }
}
