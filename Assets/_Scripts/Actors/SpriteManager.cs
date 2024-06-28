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
    private Turret owner;
    private Color eatenColor = Color.red;
    private Color consumerColor = Color.yellow;
    private Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        startScale = radiusSpriteRenderer.transform.localScale;
        UpgradeDisplay.OnUpgradeDisplay += ShowRadius;
        CombineManager.OnPossibleConsume += CombineManager_OnPossibleConsume;
        CombineManager.OnNoConsume += CombineManager_OnNoConsume;
        ShowSprites(currentSpriteGroup);
        owner = GetComponent<Turret>();
        if(owner != null )
        {
            owner.OnDeactivate += Owner_OnDeactivate;
            owner.OnActivate += Owner_OnActivate;
        }
    }

    private void CombineManager_OnNoConsume(GameObject consumer, GameObject eaten)
    {
        ChangeSpriteColor(originalColor);
    }

    private void CombineManager_OnPossibleConsume(GameObject consumer, GameObject eaten)
    {
        originalColor = midSpriteRenderer.color;
        if(consumer == owner.gameObject)
        {
            ChangeSpriteColor(consumerColor);
        } 
        else if(eaten == owner.gameObject)
        {
            ChangeSpriteColor(eatenColor);
        }
    }

    private void Owner_OnActivate()
    {
        ShowSpriteCanPlace(true);
    }

    private void Owner_OnDeactivate()
    {
        ShowSpriteCanPlace(false);
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

    void ChangeSpriteColor(Color color)
    {
        midSpriteRenderer.color = new Color(color.r, color.g, color.b, originalColor.a);
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
        UpgradeDisplay.OnUpgradeDisplay -= ShowRadius;
        CombineManager.OnPossibleConsume -= CombineManager_OnPossibleConsume;
        CombineManager.OnNoConsume -= CombineManager_OnNoConsume;
        if (owner != null)
        {
            owner.OnDeactivate -= Owner_OnDeactivate;
            owner.OnActivate -= Owner_OnActivate;
        }
    }
}
