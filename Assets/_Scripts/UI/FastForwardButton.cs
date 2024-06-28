using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour
{
    public delegate void SpeedChange();
    public static event SpeedChange OnFastForward;
    public static event SpeedChange OnNormal;
    public static event SpeedChange OnFail;

    //private bool m_IsPointerDown = false;
    private float originalSpeed = 0f;
    private bool toggle = false;

    private void Start()
    {
        
    }

    public void ToggleSpeed()
    {
        if (Time.timeScale == 0)
        {
            OnFail?.Invoke();
            return;
        }

        toggle = !toggle;
        if(toggle)
        {
            FastForward();
        } else
        {
            ReturnToNormal();
        }
        
    }


    private void FastForward()
    {
        
        originalSpeed = Time.timeScale;
        Time.timeScale = 2f * originalSpeed;
        OnFastForward?.Invoke();


    }

    void ReturnToNormal()
    {
        Time.timeScale = originalSpeed;
        OnNormal?.Invoke();
    }
}
