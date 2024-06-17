using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager Instance;
    public GameObject toolTip;
    public TextMeshProUGUI toolTipMainText;
    public TextMeshProUGUI toolTipTitleText;

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

        PrefabPortrait.OnPortraitHover += ActorPortrait_OnPortraitHover;
    }

    private void ActorPortrait_OnPortraitHover(Vector2 position, string name, string desc, bool isCursorIn)
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


    private void FixedUpdate()
    {
        if (toolTip)
        {
             toolTip.transform.position = Input.mousePosition;
        }

    }

    public void ShowToolTip(Vector2 pos, string title, string mainText)
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(true);
            toolTipTitleText.text = title;
            toolTipMainText.text = mainText;
            toolTip.transform.position = pos;
        }
    }

    public void HideToolTip()
    {
        if (toolTipMainText != null && toolTip != null)
        {
            toolTip.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        PrefabPortrait.OnPortraitHover -= ActorPortrait_OnPortraitHover;
    }
}
