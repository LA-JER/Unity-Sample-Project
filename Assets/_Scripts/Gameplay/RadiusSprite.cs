using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RadiusSprite : MonoBehaviour
{
    [SerializeField] private StatManager statManager;
    [SerializeField] private SpriteRenderer radiusSpriteRenderer;

    private Vector2 startScale= Vector2.zero;
    private bool showRadius = true;

    private void Awake()
    {
        //UpgradeNodeManager.OnShowUpgrades += UpgradeNodeManager_OnShowUpgrades;
        UpgradeDisplay.OnUpgradeDisplay += ShowRadius;
    }

    private void ShowRadius(bool show)
    {
        this.showRadius = show;
    }

    // Start is called before the first frame update
    void Start()
    {
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (showRadius)
        {
            radiusSpriteRenderer.enabled = true;
            radiusSpriteRenderer.transform.localScale = startScale * new Vector2(statManager.GetStat(StatManager.Stat.range), statManager.GetStat(StatManager.Stat.range));
        } else
        {
            radiusSpriteRenderer.enabled = false;
        }
        
    }

    private void OnDestroy()
    {
        //UpgradeNodeManager.OnShowUpgrades -= UpgradeNodeManager_OnShowUpgrades;
        UpgradeDisplay.OnUpgradeDisplay += ShowRadius;
    }
}
