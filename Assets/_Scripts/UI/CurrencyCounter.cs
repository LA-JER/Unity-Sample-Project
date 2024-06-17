using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;

public class CurrencyCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string pretext = "Gold : ";

    private void Awake()
    {
        CurrencyManager.onCurrencyChange += CurrencyManager_onCurrencyChange;
    }

    private void CurrencyManager_onCurrencyChange(int newValue)
    {
        text.text = pretext + newValue;
    }


    private void OnDestroy()
    {
        CurrencyManager.onCurrencyChange -= CurrencyManager_onCurrencyChange;
    }
}
