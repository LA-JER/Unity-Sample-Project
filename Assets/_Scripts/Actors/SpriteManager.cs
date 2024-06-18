using System.Collections;
using UnityEngine;
using static TurretSpriteGroup;

public class SpriteManager : MonoBehaviour
{
    [SerializeField] private StatManager statManager;
    [SerializeField] private SpriteRenderer topSpriteRenderer;
    [SerializeField] private SpriteRenderer midSpriteRenderer;
    [SerializeField] private SpriteRenderer bottomSpriteRenderer;
    [SerializeField] private SpriteRenderer radiusSpriteRenderer;
    [SerializeField] private TurretSpriteGroup currentSpriteGroup;

    public float colorAlphaValue = .33f;

    private Vector2 startScale = Vector2.zero;
    private bool showRadius = true;

    // Start is called before the first frame update
    void Start()
    {
        startScale = radiusSpriteRenderer.transform.localScale;
        UpgradeDisplay.OnUpgradeDisplay += ShowRadius;
        ShowSprites(currentSpriteGroup);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (showRadius)
        {
            radiusSpriteRenderer.enabled = true;
            radiusSpriteRenderer.transform.localScale = startScale * new Vector2(statManager.GetStat(StatManager.Stat.range), statManager.GetStat(StatManager.Stat.range));
        }
        else
        {
            radiusSpriteRenderer.enabled = false;
        }

    }

    public void ShowSpriteCanPlace(bool canPlace)
    {
        Color topColor = topSpriteRenderer.color;
        Color midColor = midSpriteRenderer.color;
        Color bottomColor = bottomSpriteRenderer.color;

        if (canPlace)
        {
            topSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, 1);
            midSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, 1);
            bottomSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, 1);
        } else
        {
            topSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, colorAlphaValue);
            midSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, colorAlphaValue);
            bottomSpriteRenderer.color = new Color(topColor.r, topColor.g, topColor.b, colorAlphaValue);
        }
    }

    public void ShowSprites(TurretSpriteGroup spriteGroup)
    {
        if (spriteGroup == null) return;
        foreach(var turretSprite in spriteGroup.turretSprites)
        {

            //if the turretSprite's sprite is null, keep the same current sprite
            if (turretSprite.Value == null) continue;
            switch (turretSprite.Key)
            {
                case TurretSpriteType.Top:
                    topSpriteRenderer.sprite = turretSprite.Value;
                    break;
                case TurretSpriteType.Bottom:
                    bottomSpriteRenderer.sprite= turretSprite.Value;
                    break;
                case TurretSpriteType.Middle:
                    midSpriteRenderer.sprite = turretSprite.Value;
                    break;

            }
        }
    }

    private void ShowRadius(bool show)
    {
        this.showRadius = show;
    }

    private void OnDestroy()
    {
        //UpgradeNodeManager.OnShowUpgrades -= UpgradeNodeManager_OnShowUpgrades;
        UpgradeDisplay.OnUpgradeDisplay += ShowRadius;
    }
}
