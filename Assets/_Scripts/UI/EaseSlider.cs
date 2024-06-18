using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EaseSlider : MonoBehaviour
{
    [SerializeField] private Slider frontSlider;
    [SerializeField] private Slider backSlider;
    public bool startsFull = true;
    private float maxValue = 1;

    private float currentValue = 1;
    private float lerpSpeed = 0.02f;

    public void Initialize(float max)
    {
        if (frontSlider != null && backSlider != null)
        {
            //Debug.Log("initialzing health slider to: " + max);
            maxValue = max;
            frontSlider.maxValue = max;
            backSlider.maxValue = max;

            if (startsFull)
            {
                frontSlider.value = max;
                backSlider.value = max;
                currentValue = max;
            }
            else
            {
                frontSlider.value = 0;
                backSlider.value = 0;
                currentValue = 0;
            }



        }
    }

    // Update is called once per frame
    void Update()
    {
        if (frontSlider != null && backSlider != null)
        {
            if (startsFull)
            {
                if (frontSlider.value != Mathf.Clamp(currentValue, 0, maxValue))
                {
                    frontSlider.value = Mathf.Clamp(currentValue, 0, maxValue);
                }
                if (frontSlider.value != backSlider.value)
                {
                    backSlider.value = Mathf.Lerp(backSlider.value, currentValue, lerpSpeed);
                }
            }
            else
            {
                if (backSlider.value != Mathf.Clamp(currentValue, 0, maxValue))
                {
                    backSlider.value = Mathf.Clamp(currentValue, 0, maxValue);
                }
                if (frontSlider.value != backSlider.value)
                {
                    frontSlider.value = Mathf.Lerp(frontSlider.value, currentValue, lerpSpeed);
                }

            }


        }
    }

    public void SetCurrentValue(float current)
    {
        currentValue = current;
    }

    public void SetMaxValue(float max)
    {
        maxValue = max;
        frontSlider.maxValue = max;
        backSlider.maxValue = max;
    }

    public void ForceComplete()
    {
        frontSlider.value = maxValue;
        backSlider.value = maxValue;
    }
}
