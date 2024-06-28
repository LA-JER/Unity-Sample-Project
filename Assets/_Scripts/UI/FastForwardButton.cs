using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FastForwardButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void SpeedChange();
    public static event SpeedChange OnFastForward;
    public static event SpeedChange OnNormal;

    //private bool m_IsPointerDown = false;
    private float originalSpeed = 0f;

    public void OnPointerDown(PointerEventData eventData)
    {
        originalSpeed = Time.timeScale;
        //m_IsPointerDown = true;
        FastForward();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       // m_IsPointerDown = false;
        ReturnToNormal();
    }


    private void FastForward()
    {
        Time.timeScale = 2f * originalSpeed;
        OnFastForward?.Invoke();


    }

    void ReturnToNormal()
    {
        Time.timeScale = originalSpeed;
        OnNormal?.Invoke();
    }
}
