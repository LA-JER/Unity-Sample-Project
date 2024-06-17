using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class WaveCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private string pretext = "Wave : ";

    private void Awake()
    {
        WaveManager.OnConfigStart += WaveManager_OnWaveStart;
    }

    private void WaveManager_OnWaveStart(WaveConfig config)
    {
        text.text = pretext + config.waveNumber;
    }


    private void OnDestroy()
    {
        WaveManager.OnConfigStart -= WaveManager_OnWaveStart;
    }
}
