using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrefabPortrait : UIPortrait, IEndDragHandler, IBeginDragHandler, IDragHandler
{
    public delegate void PortraitDrag(PrefabPortrait source, GameObject prefab, Vector2 portraitPosition);
    public event PortraitDrag OnPortraitDrag;

    public delegate void PortraitEndDrag();
    public event PortraitEndDrag onPortraitEndDrag; 

    public delegate void PortraitClickHandler(PrefabPortrait source, GameObject prefab);
    public event PortraitClickHandler OnPortraitClick;

    [SerializeField] private Image greyOut;
    [SerializeField] private GameObject prefab;
    
    //private bool isDragging = false;

    public int winsNeeded = 0;
    public int maxPurchases = 999;

    //private bool canBuy = true;
    private void Start()
    {
        if(prefab == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name}'s prefab was empty! Are you sure this is intended?");
            return;
        }
        

        IHoverInfo tippable = prefab.GetComponent<IHoverInfo>();
        if(tippable == null)
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name} could not find the IShowToolTip interface of gameobject {prefab}");
            return;
        }
        
        //display the information of the prefab
        icon.sprite = tippable.GetIcon();
        actorName = tippable.GetName();
        actorDesc = tippable.GetDescription();

        IPurchaseAble purchase = prefab.GetComponent<IPurchaseAble>();
        if(purchase != null)
        {
            price = purchase.GetPrice();
        }
                 
         
        if(greyOut != null)
        {
            greyOut.enabled = false;
        }
    }

    public override void Click()
    {
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} with prefab {prefab} was clicked!");
        OnPortraitClick?.Invoke(this, prefab);
    }

    public void GreyOut(bool disable)
    {
        greyOut.enabled = disable;
        button.enabled = !disable;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        onPortraitEndDrag?.Invoke();
        //isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debugger.Log(Debugger.AlertType.Warning, $"Start Drag!");
        OnPortraitDrag?.Invoke(this, prefab, transform.position);
        //isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debugger.Log(Debugger.AlertType.Warning, $" Drag!");
    }

    public GameObject GetPrefab()
    {
        return prefab;
    }
}
