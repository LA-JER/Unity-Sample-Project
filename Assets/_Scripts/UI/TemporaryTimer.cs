using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TemporaryTimer : MonoBehaviour
{
    [SerializeField] private Slider slider;

   

    public void Initialize(float maxValue)
    {
        slider.maxValue = maxValue;
        slider.value = maxValue;
        slider.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DecrementSlider();
    }

    void DecrementSlider()
    {
        slider.value -= Time.fixedDeltaTime;

        if (slider.value <= 0)
        {
            Destroy(gameObject);
        }
    }
}
