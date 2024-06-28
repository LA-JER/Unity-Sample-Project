using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;

    public Canvas displayCanvas;
    public GameObject toolTip;
    public RectTransform view;
    public TextMeshProUGUI toolTipMainText;
    public TextMeshProUGUI toolTipTitleText;
    public float xOffset = 175f;
    public float yOffset = 175f;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        UIPortrait.OnPortraitHover += UIPortrait_OnPortraitHover;
    }

    private void UIPortrait_OnPortraitHover(Vector2 position, string name, string desc, bool isCursorIn)
    {
        if(isCursorIn)
        {
            AdjustAnchorPosition();
            ShowToolTip(position, name, desc);
        }
        else
        {
            HideToolTip();
        }
    }

    private void Start()
    {
        toolTip.SetActive(false);
    }


    private void Update()
    {
        if (toolTip.activeSelf)
        {
            TrackPosition();
        }

    }

    void TrackPosition()
    {
        if (toolTip != null)
        {
            
            Vector2 screenPoint = Input.mousePosition;
            //Vector2 clampedPoint = ClampToCanvasEdge(view, screenPoint);

            //toolTip.transform.position = clampedPoint + new Vector2(-2, 0);
            toolTip.transform.position = screenPoint;
            
        }
    }

    private void ShowToolTip(Vector2 pos, string title, string mainText)
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(true);
            toolTipTitleText.text = title;
            toolTipMainText.text = mainText;
            //toolTip.transform.position = ClampToCanvasEdge(view, pos);
            //toolTip.transform.position = pos;
        }
    }

    private void HideToolTip()
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }

    void AdjustAnchorPosition()
    {
        Vector2 adjustment = Vector2.zero;
        Vector2 screenPoint = Input.mousePosition;

        adjustment.x = IsPointOnRightSide(screenPoint) ? -xOffset :  xOffset;
        adjustment.y = IsPointOnTopSide(screenPoint) ? -yOffset : yOffset;

        view.anchoredPosition = adjustment;
    }

    private bool IsPointOnRightSide(Vector2 screenPoint)
    {
        float screenWidth = Screen.width;
        return screenPoint.x > screenWidth / 2;
    }

    private bool IsPointOnTopSide(Vector2 screenPoint)
    {
        float screenHeight = Screen.height;
        return screenPoint.y > screenHeight / 2;
    }

    private void OnDestroy()
    {
        PrefabPortrait.OnPortraitHover -= UIPortrait_OnPortraitHover;
    }
}
