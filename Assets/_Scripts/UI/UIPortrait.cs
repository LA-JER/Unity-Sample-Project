using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UIPortrait : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void PortraitHover(Vector2 position, string name, string desc, bool isCursorIn);
    public static event PortraitHover OnPortraitHover;

    public Image icon;
    public Transform toolTipSpawn;
    public string actorName;
    public string actorDesc;
    public int price = 0;


    public abstract void Click();

    public void OnPointerEnter(PointerEventData eventData)
    {


        OnPortraitHover?.Invoke(toolTipSpawn.position, actorName + " - $" + price, actorDesc, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnPortraitHover?.Invoke(toolTipSpawn.position, actorName, actorDesc, false);
    }

    private void OnDestroy()
    {
        OnPortraitHover?.Invoke(toolTipSpawn.position, actorName, actorDesc, false);
    }

    private void OnDisable()
    {
        OnPortraitHover?.Invoke(toolTipSpawn.position, actorName, actorDesc, false);
    }
}
