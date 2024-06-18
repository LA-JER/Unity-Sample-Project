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
    public float xMinModifier = 0f;
    public float xMaxModifier = .75f;

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
        if (toolTip)
        {
            //TrackPosition();
        }

    }

    void TrackPosition()
    {
        if (toolTip != null)
        {
            
                Vector2 screenPoint = Input.mousePosition;
                Vector2 clampedPoint = ClampToCanvasEdge(view, screenPoint);

            toolTip.transform.position = clampedPoint;

            
        }
    }

    public void ShowToolTip(Vector2 pos, string title, string mainText)
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(true);
            toolTipTitleText.text = title;
            toolTipMainText.text = mainText;
            toolTip.transform.position = ClampToCanvasEdge(view, pos);
            //toolTip.transform.position = pos;
        }
    }

    public void HideToolTip()
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }

    Vector2 ClampToCanvasEdge(RectTransform view, Vector2 screenPoint)
    {

        Vector3 max1 = displayCanvas.pixelRect.max;
        screenPoint.x = Mathf.Clamp(screenPoint.x, 0 + (view.rect.size.x * xMinModifier), max1.x - (view.rect.size.y * xMaxModifier));
        screenPoint.y = Mathf.Clamp(screenPoint.y, 0 , max1.y);
        return screenPoint;
    }

    private void OnDestroy()
    {
        PrefabPortrait.OnPortraitHover -= UIPortrait_OnPortraitHover;
    }
}
