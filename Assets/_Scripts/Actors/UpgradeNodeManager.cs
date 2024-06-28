using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeNodeManager : MonoBehaviour
{
    public delegate void UpgradeHandler(List<UpgradeNode> upgradeNodes, GameObject owner, Vector2 worldPosition, string title);
    public static event UpgradeHandler OnShowUpgrades;

    [SerializeField] private List<UpgradeNode> startNodes = new List<UpgradeNode>();
    
    [SerializeField] private Transform spawn;

    public List<UpgradeNode> availableUpgrades = new List<UpgradeNode>();

    //private Collider2D collide;
    private Turret owner;
    private InputHandler handler;

    private void Awake()
    {
        UpgradeDisplay.OnBuyUpgrade += UpgradeDisplay_OnBuyUpgrade;
        handler = GetComponent<InputHandler>();
        if(handler != null)
        {
            owner = handler.GetOwner();
            handler.OnMouseUpEvent += Handler_OnMouseUp;
        }
        
    }

    private void Handler_OnMouseUp()
    {
        IHoverInfo hoverInfo = owner.GetComponent<IHoverInfo>();
        if (hoverInfo != null)
        {
            OnShowUpgrades?.Invoke(availableUpgrades, owner.gameObject, spawn.position, hoverInfo.GetName());
        }
    }

    private void Start()
    {
        if(startNodes != null)
        {
            foreach(UpgradeNode startNode in startNodes)
            {
                availableUpgrades.Add(startNode);
            }
            
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} ddi not have an upgrade path, did you forget to set one?");
        }
        //collide = GetComponent<Collider2D>();

    }
    private void UpgradeDisplay_OnBuyUpgrade(GameObject owner, UpgradeNode chosenUpgrade)
    {
        if(owner != null && owner == this.owner.gameObject)
        {
            ApplyUpgrade(chosenUpgrade);
            PrepareNextUpgrades(chosenUpgrade);
        }
    }


    void ApplyUpgrade(UpgradeNode purchasedUpgrade)
    {
        purchasedUpgrade.ApplyUpgrade(owner.gameObject);
    }

    void PrepareNextUpgrades(UpgradeNode purchasedUpgrade)
    {
        availableUpgrades.Clear();
        //add the next possible upgrades
        List<UpgradeNode> nextNodes = purchasedUpgrade.nextUpgrades;
        foreach (UpgradeNode upgradeNode in nextNodes)
        {
            availableUpgrades.Add(upgradeNode);
        }
    }


    private void OnDestroy()
    {
        UpgradeDisplay.OnBuyUpgrade -= UpgradeDisplay_OnBuyUpgrade;
        if(handler != null)
        {
            handler.OnMouseUpEvent += Handler_OnMouseUp;
        }
        
    }
}
