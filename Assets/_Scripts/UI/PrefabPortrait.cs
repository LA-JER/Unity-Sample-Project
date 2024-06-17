using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PrefabPortrait : UIPortrait
{
    public delegate void PortraitClickHandler(GameObject prefab);
    public event PortraitClickHandler OnPortraitClick;
    [SerializeField] private GameObject prefab;


    private void Start()
    {
        if(prefab != null)
        {
            IHoverInfo tippable = prefab.GetComponent<IHoverInfo>();
            if(tippable != null)
            {
                icon.sprite = tippable.GetIcon();
                actorName = tippable.GetName();
                actorDesc = tippable.GetDescription();

                IPurchaseAble purchase = prefab.GetComponent<IPurchaseAble>();
                if(purchase != null)
                {
                    price = purchase.GetPrice();
                }
                 
            } else
            {
                Debugger.Log(Debugger.AlertType.Warning, $"{name} could not find the IShowToolTip interface of gameobject {prefab}");
            }
        } else
        {
            Debugger.Log(Debugger.AlertType.Warning, $"{name}'s prefab was empty! Are you sure this is intended?");
        }
    }

    public override void Click()
    {
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} with prefab {prefab} was clicked!");
        OnPortraitClick?.Invoke(prefab);
    }
}
