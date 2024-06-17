using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public delegate void TryBuyHandler(bool successful);
    public static event TryBuyHandler OnTryBuy;

    public delegate void TryPlaceHandler(bool successful);
    public static event TryPlaceHandler OnTryPlace;

    public delegate void RefundHandler();
    public static event RefundHandler OnRefund;

    public delegate void BuyTurret(Turret turret, int cost);
    public static event BuyTurret OnBuyTurret;

    public static Shop Instance;

    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject portraitPrefab;
    [SerializeField] private KeyCode refundkey;
    [SerializeField] private List<PrefabPortrait> portraits = new List<PrefabPortrait>();

    private GameManager gameManager;
    private CurrencyManager currencyManager;
    private Turret currentPurchase;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        } else
        {
            Destroy(this);
        }
        PauseMenu.onPauseToggle += PauseMenu_onPauseToggle;
        
        
    }

    //automatically refunds turret in hand when pausing
    private void PauseMenu_onPauseToggle(bool isPaused)
    {
        if(isPaused)
        {
            RefundTurret(currentPurchase, true);
            currentPurchase = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        currencyManager = CurrencyManager.instance;
        foreach (PrefabPortrait p in portraits)
        {
            p.OnPortraitClick += Portrait_OnPortraitClick;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckMouseDown();
        MoveActor();
        CheckIfRefund();
    }

    //First, checks if the player has the funds to purchase, then creates
    //an instance of the desired prefab and prepares for it to follow the cursor
    private void Portrait_OnPortraitClick(GameObject prefab)
    {
        if(currencyManager != null)
        {
            IPurchaseAble purchaseable = prefab.GetComponent<IPurchaseAble>();
            if(purchaseable != null)
            {
                int price = purchaseable.GetPrice();
                if (currencyManager.CanBuy(price))
                {
                    OnTryBuy?.Invoke(true);
                    currencyManager.Buy(price);
                    GameObject prefabInstance = Instantiate(prefab, gameManager.GetTurretHolder());

                    
                    Turret turret = prefabInstance.GetComponent<Turret>();
                    currentPurchase = turret;
                    OnBuyTurret?.Invoke(turret, price);

                    
                } else
                {
                    Debugger.Log(Debugger.AlertType.Verbose  , $"Tried to buy {prefab} but not enough funds!");
                    OnTryBuy?.Invoke(false);
                }
            } else
            {
                Debugger.Log(Debugger.AlertType.Error, $"{name} tried to purchase {prefab} but it did not implement IPurchaseable!");
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} tried to find currency manager but nothing was found!");
        }
    }

    //if a turret is currently in hand, places down and enables the turret only if
    //this is valid position, that is, not over the enemy path
    void CheckMouseDown()
    {
        if (currentPurchase != null )
        {
            Vector3 mouseWorldPOS = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPOS = new Vector3(mouseWorldPOS.x, mouseWorldPOS.y, -1);

            //changes opacity of turret in hand to tell the player if this is an invalid location
            SpriteManager spriteRenderer = currentPurchase.GetComponent<SpriteManager>();
           
            if (spriteRenderer != null)
            {
               
                   spriteRenderer.ShowSpriteCanPlace(CanPlaceHere(mouseWorldPOS));
                
            }

            //handles placement logic
            if (Input.GetMouseButtonDown(0))
            {
                if (CanPlaceHere(mouseWorldPOS))
                {
                    IPurchaseAble purchaseAble = currentPurchase.GetComponent<IPurchaseAble>();
                    if (purchaseAble != null)
                    {
                        purchaseAble.Activate();
                    }
                    currentPurchase = null;
                    OnTryPlace?.Invoke(true);

                }
                else
                {
                    OnTryPlace?.Invoke(false);
                }

            }
        }
    }

    void CheckIfRefund()
    {
        if(currentPurchase != null  )
        {
            if (Input.GetKey(refundkey))
            {
                RefundTurret(currentPurchase, true);
                currentPurchase = null;
            }
        }
    }

    public static void RefundTurret(Turret turret, bool refundFull)
    {
        if(turret != null)
        {
            if(refundFull)
            {
                CurrencyManager.instance.Refund(turret.GetPrice());
            }
            else
            {
                int totalCost = turret.GetTotalSpent();
                int adjustedRefund = (int)(totalCost * GameManager.Instance.GetRefundPercent());
                CurrencyManager.instance.Refund(adjustedRefund);
            }
            OnRefund?.Invoke();
            Destroy(turret.gameObject);
        }
    }

    void MoveActor()
    {
        if(currentPurchase != null )
        {
            Vector2 mouseWorldPOS = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPurchase.transform.position = mouseWorldPOS;
            
        }
    }

    bool CanPlaceHere(Vector2 targetPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(targetPosition, Vector3.forward);
        if(hit.collider != null)
        {
          //  Debug.Log(hit.collider);
        }
        if (hit.collider != null && hit.collider.CompareTag("Path"))
        {
            return false;
        }
        return true;
    }


    private void OnDestroy()
    {
        foreach (PrefabPortrait p in portraits)
        {
            p.OnPortraitClick -= Portrait_OnPortraitClick;
        }
    }
}
