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
    [Tooltip("Works with Projectile Movements (max 1) and Projectile Hit Effects (no max)")]
    public List<GameObject> projectileModulesPrefabs = new List<GameObject>();
    public List<GameObject> turretItemsPrefabs = new List<GameObject>();

    public override void ApplyUpgrade(GameObject owner)
    {
        if(owner != null)
        {

            //applies the stat changes
            BuffManager buffManager = owner.GetComponent<BuffManager>();
            if(buffManager != null)
            {
                
                foreach(var entry in statDictionary)
                {
                    Debugger.Log(Debugger.AlertType.Verbose, $"applying {entry.Value} to {entry.Key} on {owner}!");
                    buffManager.ApplyPermanentBuff(entry.Key, entry.Value.operation, entry.Value.modifier);
                }
                
            }

            //adds new modules tothe projectile, such as movement type and hit effects
            DamageElementManager projectileManager = owner.GetComponent<DamageElementManager>();
            if(projectileManager != null)
            {
                
                 projectileManager.AddNewModules(projectileModulesPrefabs);
                
            }

            SpriteManager spriteManager = owner.GetComponent<SpriteManager>();
            if(spriteManager != null)
            {
                spriteManager.ShowSprites(spriteGroup);
            }

            ItemManager itemManager = owner.GetComponent<ItemManager>();
            if(itemManager != null)
            {
                itemManager.AddItemsFromPrefabs(turretItemsPrefabs);
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} tried to apply upgrade but the given actor was null!");
        }
    }

    public override void RemoveUpgrade(GameObject owner)
    {
        throw new NotImplementedException();
    }
}
