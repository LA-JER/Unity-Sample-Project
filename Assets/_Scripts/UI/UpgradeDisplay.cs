using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeDisplay : MonoBehaviour
{
    public delegate void UpgradeHandler(GameObject owner, UpgradeNode chosenUpgrade);
    public static event UpgradeHandler OnBuyUpgrade;

    public delegate void TryBuyHandler(bool successful);
    public static event TryBuyHandler OnTryBuy;

    public delegate void EnableUpgradeDisplay(bool isShowing);
    public static event EnableUpgradeDisplay OnUpgradeDisplay;

    public static UpgradeDisplay instance;

    [SerializeField] private Canvas displayCanvas;
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI details;
    [SerializeField] private TextMeshProUGUI sell;
    [SerializeField] private GameObject display;
    [SerializeField] private RectTransform view;
    [SerializeField] private RectTransform dropOptions;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject portraitPrefab;
    [SerializeField] private List<ScriptablePortrait> portraits = new List<ScriptablePortrait>();

    private CurrencyManager currencyManager;
    private UpgradeNode chosenUpgrade;
    private Turret currentTurret;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        UpgradeNodeManager.OnShowUpgrades += ShowUpgrades;
        Shop.OnRefund += Shop_OnRefund;
    }

    //hide the status dispaly on refunds
    private void Shop_OnRefund()
    {
        display.SetActive(false);
    }

    private void ShowUpgrades(List<UpgradeNode> upgradeNodes, GameObject owner, Vector2 p, string titlw )
    {
        ShowCurrentTargetingStyle();
        UpdateUpgrades(upgradeNodes);

        title.text = titlw;
        display.SetActive(!display.activeSelf);
        display.transform.position = Camera.main.WorldToScreenPoint(p);
        OnUpgradeDisplay(display.activeSelf);

        Turret t = owner.GetComponent<Turret>();
        if( t != null)
        {
            currentTurret = t;
        }
        
    }

    //values that can change 
    void ShowDetails()
    {
        if (details == null) return;
        if (currentTurret == null) return;
        float dmg = currentTurret.GetTotalDamage();
        int kills = currentTurret.GetKills();

        details.text = $" DMG : {NumberShorthand.FormatNumber(dmg)}  KILLS : {NumberShorthand.FormatNumber(kills)}";

        if(sell != null)
        {
            int totalCost = currentTurret.GetTotalSpent();
            int adjustedRefund = (int)(totalCost * GameManager.Instance.GetRefundPercent());

            sell.text = $"SELL : {NumberShorthand.FormatNumber(adjustedRefund)}";
        }
    }

    private void UpdateUpgrades(List<UpgradeNode> upgradeNodes)
    {
        if (upgradeNodes != null)
        {
            for (int i = 0; i < portraits.Count; i++)
            {
                if (i < upgradeNodes.Count)
                {
                    if (portraits[i].scriptable != upgradeNodes[i])
                    {
                        portraits[i].Initialize(upgradeNodes[i]);
                    }
                    portraits[i].gameObject.SetActive(true);
                }
                else
                {
                    portraits[i].gameObject.SetActive(false);
                }
            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {
        currencyManager = CurrencyManager.instance;
        foreach (ScriptablePortrait p in portraits)
        {
            p.OnPortraitClick += OnPortraitClick;
        }
        display.SetActive(false);
    }

    private void Update()
    {
        CheckHide();
        TrackPosition();
        //ShowDetails();
    }

    private void FixedUpdate()
    {
        //TrackPosition();
        ShowDetails();
    }

    public void Sell()
    {
        Shop.RefundTurret(currentTurret, false);
    }

    public void ChangeSearchType()
    {
        if(dropdown != null && currentTurret != null)
        {
            int chosenStyle = dropdown.value;
            Targeting.TargetingStyle style = (Targeting.TargetingStyle)chosenStyle;
            
            Turret turret = currentTurret.GetComponent<Turret>();
            if(turret != null)
            {
                turret.targetingStyle = style;
            }
        }
        
    }

    private void ShowCurrentTargetingStyle()
    {
        if (dropdown != null && currentTurret != null)
        {
                dropdown.value = (int)currentTurret.targetingStyle;
            
        }
    }

    //called when the player attempts to buy the upgrade
    private void OnPortraitClick(ScriptableObject scriptable)
    {
        if (currencyManager != null)
        {

            if (scriptable is IPurchaseAble)
            {
                IPurchaseAble purchase = (IPurchaseAble)scriptable;
                int price = purchase.GetPrice();
                if (currencyManager.CanBuy(price))
                {

                    

                    if (scriptable is UpgradeNode)
                    {
                        UpgradeNode upgradeNode = (UpgradeNode)scriptable;
                        chosenUpgrade = upgradeNode;
                        //PrepareNextUpgrades(upgradeNode);
                        OnTryBuy?.Invoke(true);
                        OnBuyUpgrade?.Invoke(currentTurret.gameObject, upgradeNode);
                        currencyManager.Buy(price);
                        Refresh();
                    }


                }
                else
                {
                    OnTryBuy?.Invoke(false);
                    Debugger.Log(Debugger.AlertType.Verbose, $"Tried to buy {scriptable} but not enough funds!");

                }
            }
            else { Debugger.Log(Debugger.AlertType.Error, $"{name} tried to purchase {scriptable} but it did not implement IPurchaseable!"); }
        }
        else { Debugger.Log(Debugger.AlertType.Error, $"{name} tried to find currency manager but nothing was found!"); }
    }

    void CheckHide()
    {
        if(view != null && dropOptions != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIElement(view) && !IsPointerOverUIElement(dropOptions))
                {
                    display.SetActive(false);
                    OnUpgradeDisplay?.Invoke(false);
                }
            }
        }
    }

    void TrackPosition()
    {
        if(currentTurret != null)
        {
            if(display != null && display.activeSelf)
            {
                    Vector2 screenPoint = Camera.main.WorldToScreenPoint(currentTurret.transform.position);
                    Vector2 clampedPoint = ClampToCanvasEdge(view, screenPoint);

                    display.transform.position = clampedPoint;
                
            }
        }
    }

    private bool IsPointerOverUIElement(RectTransform rectTransform)
    {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        return rectTransform.rect.Contains(localMousePosition);
    }

    private void Refresh()
    {
        if(display != null && chosenUpgrade != null)
        {
            if(display.activeSelf)
            {
                UpdateUpgrades(chosenUpgrade.nextUpgrades);
            }
        }
    }


    private ScriptablePortrait AddPortrait()
    {
        if (portraitPrefab != null && content != null)
        {
            GameObject portraitOBJ = Instantiate(portraitPrefab, content);
            ScriptablePortrait purchasePortrait = portraitOBJ.GetComponent<ScriptablePortrait>();
            if (purchasePortrait != null)
            {
                portraits.Add(purchasePortrait);
                purchasePortrait.OnPortraitClick += OnPortraitClick;
                return purchasePortrait;
            }
        }
        return null;
    }

    private void RemovePortrait(ScriptablePortrait p)
    {
        if (p != null)
        {
            if (portraits.Contains(p))
            {
                portraits.Remove(p);
                p.OnPortraitClick -= OnPortraitClick;
                Destroy(p);
            }
        }
    }

    Vector2 ClampToCanvasEdge(RectTransform view, Vector2 screenPoint)
    {

        Vector3 max1 = displayCanvas.pixelRect.max;
        screenPoint.x = Mathf.Clamp(screenPoint.x, 0+(view.rect.size.x * 0.75f), max1.x);
        screenPoint.y = Mathf.Clamp(screenPoint.y, 0 + (view.rect.size.y * 0.25f), max1.y);
        return screenPoint;
    }

    private void OnDestroy()
    {
        foreach (ScriptablePortrait p in portraits)
        {
            p.OnPortraitClick -= OnPortraitClick;
        }
        UpgradeNodeManager.OnShowUpgrades -= ShowUpgrades;
        Shop.OnRefund -= Shop_OnRefund;
    }
}
