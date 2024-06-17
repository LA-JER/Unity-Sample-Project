using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptablePortrait : UIPortrait
{
    public delegate void ScriptPortraitClickHandler(ScriptableObject scriptable);
    public event ScriptPortraitClickHandler OnPortraitClick;
    public ScriptableObject scriptable;


    private void Start()
    {
        Initialize(scriptable);
    }

    public override void Click()
    {
        Debugger.Log(Debugger.AlertType.Verbose, $"{name} with prefab {scriptable} was clicked!");
        OnPortraitClick?.Invoke(scriptable);
    }

    public void Initialize( ScriptableObject scriptable)
    {
        if (scriptable != null)
        {
            this.scriptable = scriptable;
            if (scriptable is IHoverInfo)
            {
                IHoverInfo hoverInfo = (IHoverInfo)scriptable;

                icon.sprite = hoverInfo.GetIcon();
                actorName = hoverInfo.GetName();
                actorDesc = hoverInfo.GetDescription();


                if (scriptable is IPurchaseAble)
                {
                    IPurchaseAble purchaseAble = (IPurchaseAble)scriptable;
                    price = purchaseAble.GetPrice();
                }

            }
            else
            {
                Debugger.Log(Debugger.AlertType.Warning, $"{name} could not find the IShowToolTip interface of gameobject {scriptable}");
            }
        }
        else
        {
           // Debugger.Log(Debugger.AlertType.Warning, $"{name}'s prefab was empty! Are you sure this is intended?");
        }
    }
}
