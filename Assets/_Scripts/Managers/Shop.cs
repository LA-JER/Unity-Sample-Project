using AYellowpaper.SerializedCollections;
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

   // public delegate void BuyTurret(Turret turret, int cost);
   // public static event BuyTurret OnBuyTurret;

    public static Shop Instance;

    [SerializeField] private RectTransform content;
    [SerializeField] private List<RectTransform> nonPlaceable = new List<RectTransform>();
    [SerializeField] private GameObject portraitPrefab;
    [SerializeField] private KeyCode refundkey;
    [SerializeField] private List<PrefabPortrait> portraits = new List<PrefabPortrait>();
    [SerializedDictionary("Turret", "Purchase Count")]
    public SerializedDictionary<PrefabPortrait, int> purchaseCounts = new SerializedDictionary<PrefabPortrait, int>();

    private GameManager gameManager;
    
    private CurrencyManager currencyManager;
    private Turret currentPurchase;
    private int dragging = 0; //0 means no dragging, 1 means dragging started, 2 means dragging ended
    public Vector3 offset = Vector3.zero;

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
            if(currentPurchase != null)
            {
                RefundPurchase(currentPurchase, true);
                currentPurchase = null;
            }
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        currencyManager = CurrencyManager.instance;
        InitializePortraits();

        
    }

    private void P_onPortraitEndDrag()
    {
        dragging = 2;
        DragPlaceTurret();
    }

    private void P_OnPortraitDrag(PrefabPortrait source, GameObject prefab, Vector2 portraitPosition)
    {
        dragging = 1;
        BuyAndInstantiate(source, prefab, Camera.main.ScreenToWorldPoint(portraitPosition));
        
    }

    private void OnPortraitClick(PrefabPortrait source, GameObject prefab)
    {
        offset = Vector2.zero;
        BuyAndInstantiate(source, prefab);
        dragging = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (dragging == 0)
        {

            ClickPlaceTurret();
        }
        CheckPlaceTurret();
        MoveActor();
        CheckIfRefund();
    }

    //First, checks if the player has the funds to purchase, then creates
    //an instance of the desired prefab and prepares for it to follow the cursor
    void BuyAndInstantiate(PrefabPortrait source, GameObject prefab, Vector2 spawnPosition = new Vector2())
    {
        if(currencyManager == null)
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} tried to find currency manager but nothing was found!");
            return;
        }
        
        IPurchaseAble purchaseable = prefab.GetComponent<IPurchaseAble>();
        if(purchaseable == null)
        {
            Debugger.Log(Debugger.AlertType.Error, $"{name} tried to purchase {prefab} but it did not implement IPurchaseable!");
            return;
        }
            
        int price = purchaseable.GetPrice();
        if (currencyManager.CanBuy(price))
        {
            OnTryBuy?.Invoke(true);
            HandlePrefabCounts(source, 1);
            currencyManager.Buy(price);
            GameObject prefabInstance = Instantiate(prefab, gameManager.GetTurretHolder());
            if(spawnPosition != null && dragging == 1)
            {
                prefabInstance.transform.position = spawnPosition;
            }
                    
            Turret turret = prefabInstance.GetComponent<Turret>();
            currentPurchase = turret;
            //OnBuyTurret?.Invoke(turret, price);



            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            offset = prefabInstance.transform.position - worldPosition;

        } else
        {
            Debugger.Log(Debugger.AlertType.Verbose  , $"Tried to buy {prefab} but not enough funds!");
            OnTryBuy?.Invoke(false);
        }
        
        
    }

    private void HandlePrefabCounts(PrefabPortrait source, int count)
    {
        if (source == null || !purchaseCounts.ContainsKey(source)) return;

        purchaseCounts[source] += count;
        if (purchaseCounts[source] >= source.maxPurchases) 
        {
            source.GreyOut(true);
        } else
        {
            source.GreyOut(false);
        }
    }

    //if a turret is currently in hand, places down and enables the turret only if
    //this is valid position, that is, not over the enemy path
    void CheckPlaceTurret()
    {
        if (currentPurchase != null )
        {
            //changes opacity of turret in hand to tell the player if this is an invalid location
            SpriteManager spriteRenderer = currentPurchase.GetComponent<SpriteManager>();
           
            if (spriteRenderer != null)
            {
               
                   spriteRenderer.ShowSpriteCanPlace(CanPlaceHere());
                
            }
        }
    }

    void ClickPlaceTurret()
    {
        if (currentPurchase != null)
        {
            //handles placement logic
            if ((Input.GetMouseButtonDown(0)))
            {
                if (CanPlaceHere())
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


    void DragPlaceTurret()
    {
        if (currentPurchase != null)
        {
            Vector3 mouseWorldPOS = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPOS = new Vector3(mouseWorldPOS.x, mouseWorldPOS.y, -1);

            //changes opacity of turret in hand to tell the player if this is an invalid location
            SpriteManager spriteRenderer = currentPurchase.GetComponent<SpriteManager>();

            if (spriteRenderer != null)
            {

                spriteRenderer.ShowSpriteCanPlace(CanPlaceHere());

            }

            //handles placement logic
                if (CanPlaceHere())
                {
                    currentPurchase.Activate(null);
                    currentPurchase = null;
                    OnTryPlace?.Invoke(true);

                }
                else
                {
                    OnRefund?.Invoke();
                    RefundPurchase(currentPurchase, true);
                }
            
        }
    }

    void CheckIfRefund()
    {
        if(currentPurchase != null  )
        {
            if (Input.GetKey(refundkey))
            {
                RefundPurchase(currentPurchase, true);
                currentPurchase = null;
            }
        }
    }

    public void RefundPurchase(Turret purchase, bool refundFull)
    {
        if(purchase != null)
        {
            if(refundFull)
            {
                CurrencyManager.instance.Refund(purchase.GetTotalSpent());
            }
            else
            {
                int totalCost = purchase.GetTotalSpent();
                int adjustedRefund = (int)(totalCost * GameManager.Instance.GetRefundPercent());
                CurrencyManager.instance.Refund(adjustedRefund);
            }

            PrefabPortrait toDecrement = null;
            foreach (PrefabPortrait portrait in portraits)
            {
                if (portrait == null) continue;
                GameObject turretObjectPrefab = portrait.GetPrefab();
                Turret turretPrefab = turretObjectPrefab?.GetComponent<Turret>();
               if(turretPrefab.GetName() == purchase.GetName())
                {
                    toDecrement = portrait;
                    break;
                }

            }
            HandlePrefabCounts(toDecrement, -1);
            OnRefund?.Invoke();
            purchase.Destroy();
            
        }
    }

    void MoveActor()
    {
        if(currentPurchase != null )
        {
            Vector2 mouseWorldPOS = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPurchase.transform.position = mouseWorldPOS;//+ offset;
            
                if(dragging == 1)
            {
                currentPurchase.transform.position += new Vector3(offset.x, offset.y, 0);
            }
        }
    }

    bool CanPlaceHere()
    {
        //first checks if the point is not in the shop view port
        //Vector2 screenPoint = Input.mousePosition;
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(currentPurchase.transform.position);
        foreach (RectTransform rect in nonPlaceable)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, screenPoint))
            {
                return false;
            }
        }
        


        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(screenPoint);
        Vector3 origin = new Vector3(mouseWorld.x, mouseWorld.y, -1);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.forward);
        if (hit.collider != null && ( hit.collider.CompareTag("Path") || hit.collider.CompareTag("TurretSpace")))
        {
            return false;
        }
        return true;
    }


    void InitializePortraits()
    {
        foreach (PrefabPortrait p in portraits)
        {
            //listen to each portrait's events
            p.OnPortraitClick += OnPortraitClick;
            p.OnPortraitDrag += P_OnPortraitDrag;
            p.onPortraitEndDrag += P_onPortraitEndDrag;

            bool unlocked = DataHandler.GetFlagInt(DataHandler.Flag.totalWins) >= p.winsNeeded;
            p.gameObject.SetActive(unlocked);

            if (!purchaseCounts.ContainsKey(p))
            {
                purchaseCounts.Add(p, 0);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (PrefabPortrait p in portraits)
        {
            p.OnPortraitClick -= OnPortraitClick;
        }
    }
}
