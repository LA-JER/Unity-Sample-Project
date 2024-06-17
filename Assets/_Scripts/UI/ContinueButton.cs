using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContinueButton : MonoBehaviour
{
    public delegate void ContinueButtonClick();
    public static event ContinueButtonClick OnContinue;

    [SerializeField] private Image image;

    private bool canClick = true;

    private void Awake()
    {
        WaveManager.OnWaveEnd += WaveManager_OnWaveEnd;
    }

    private void WaveManager_OnWaveEnd()
    {
        canClick = true;
        EnableImage(canClick);
    }

    public void Click()
    {
        OnContinue?.Invoke();
        canClick = false;
        EnableImage(canClick);
    }

    void EnableImage(bool enable)
    {
        if (image != null)
        {
            image.enabled = enable;
        }
    }
}
